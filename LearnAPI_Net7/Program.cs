using AutoMapper;
using LearnAPI_Net7.Container;
using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Models.ViewModels.ModelsHelpers;
using LearnAPI_Net7.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();

var _jweSettings = builder.Configuration.GetSection("JWTSettings");
builder.Services.Configure<JWTSettings>(_jweSettings);


builder.Services.AddDbContext<LearnDataContaxt>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//// Enable basic Authentication
//builder.Services.AddAuthentication("BasicAuthentication")
//    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);


// Enable JWT Authentication
var _authKey = builder.Configuration.GetSection("JWTSettings:securityKey").Value;

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_authKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

//Enabling the CORS 
builder.Services.AddCors(p => p.AddPolicy("Corspolicy", build =>
{
    build.WithOrigins("https://domain1.com", "https://domain2.com").AllowAnyMethod().AllowAnyHeader();
}));

// this cors is use at controller level 
builder.Services.AddCors(p => p.AddPolicy("CorsForController", build =>
{
    build.WithOrigins("https://domain11.com").AllowAnyMethod().AllowAnyHeader();
}));

// if you want to give access to all URLs
builder.Services.AddCors(p => p.AddDefaultPolicy(build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

//builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
//{
//    options.Window = TimeSpan.FromSeconds(30);
//    options.PermitLimit = 10;
//    options.QueueLimit = 2;
//    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
//}).RejectionStatusCode=406);


// Create Projects logs and save 
string logPath = builder.Configuration.GetSection("Logging:Logpath").Value;
var _logger = new LoggerConfiguration()
    .MinimumLevel.Error().MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(logPath)
    .CreateLogger();

builder.Logging.AddSerilog(_logger);


// Adding AutoMapper 
var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper= autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);


var app = builder.Build();

//app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Corspolicy");

//app.UseCors(); // For Default CorsPolicy

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
