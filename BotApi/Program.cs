using BotApi.Models.Configurations;
using BotApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.Storage;
using Hangfire.MySql;
using System.Transactions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var botSection = builder.Configuration.GetSection("BotData");
var DataBaseSection = builder.Configuration.GetSection("DataBase");

builder.Services.Configure<BotData>(botSection);
builder.Services.Configure<DBInfo>(DataBaseSection);

var hfDbPath = builder.Configuration.GetValue<string>("HangFireDB");

builder.Services.AddHangfire(conf => conf
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage<MySqlStorage>(new (hfDbPath, new MySqlStorageOptions
    {
        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        QueuePollInterval = TimeSpan.FromSeconds(15),
        JobExpirationCheckInterval = TimeSpan.FromHours(1),
        CountersAggregateInterval = TimeSpan.FromMinutes(5),
        PrepareSchemaIfNecessary = true,
        DashboardJobListLimit = 50000,
        TransactionTimeout = TimeSpan.FromMinutes(1),
        TablesPrefix = "Hangfire"
    }))
);

builder.Services.AddHangfireServer();

builder.Services.AddCors();

builder.Services.AddDbContext<BotDbContext>(opt => opt.UseMySql("Server=localhost;Database=Bot;Uid=root;Pwd=root;", new MySqlServerVersion(new Version(5,7,24))));

builder.Services.AddTransient<BotHostService>();
builder.Services.AddTransient<NotificationService>();
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

app.UseHangfireDashboard("/dashboard");

app.MapControllers();

app.UseCors(corsBuilder =>
{
    corsBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
});

var scope = app.Services.CreateScope();
var notifService = scope.ServiceProvider.GetService<NotificationService>()!;

RecurringJob.AddOrUpdate("DailyNotification", () => notifService.SendDailyAppointments(), Cron.Daily);
RecurringJob.AddOrUpdate("NearestNotification", () => notifService.SendNearestAppointments(), () => "*/15 * * * 1-5");

app.Run();
