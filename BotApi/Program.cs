using BotApi.Models.Configurations;
using BotApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var botSection = builder.Configuration.GetSection("BotData");
var DataBaseSection = builder.Configuration.GetSection("DataBase");

builder.Services.Configure<BotData>(botSection);
builder.Services.Configure<DBInfo>(DataBaseSection);

builder.Services.AddCors();

builder.Services.AddDbContext<BotDbContext>(opt => opt.UseMySql("Server=localhost;Database=Bot;Uid=root;Pwd=root;", new MySqlServerVersion(new Version(5,7,24))));

builder.Services.AddTransient<BotHostService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseCors(corsBuilder =>
{
    corsBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
});

app.Run();
