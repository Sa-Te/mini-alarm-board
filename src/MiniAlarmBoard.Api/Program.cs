using MiniAlarmBoard.Api.Storage;
using MiniAlarmBoard.Api.Services;
using MiniAlarmBoard.Api.Models;
using MiniAlarmBoard.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAlarmStore, InMemoryAlarmStore>();
builder.Services.AddHostedService<AlarmGenerator>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseAuthorization();

app.MapHub<AlarmHub>("/hubs/alarms");

app.Run();
