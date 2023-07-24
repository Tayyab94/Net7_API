using AutoMapper;
using LearnAPI_Net7.Container;
using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Helpers;
using LearnAPI_Net7.Services;
using Microsoft.EntityFrameworkCore;

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
//builder.Services.AddNewtonsoftJson(options =>
// {
//     options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
// });
// Adding AutoMapper 
var autoMapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper= autoMapper.CreateMapper();
builder.Services.AddSingleton(mapper);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
