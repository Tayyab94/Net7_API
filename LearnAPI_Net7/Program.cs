using AutoMapper;
using LearnAPI_Net7.Container;
using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddDbContext<LearnDataContaxt>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Enable basic Authentication
builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

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
