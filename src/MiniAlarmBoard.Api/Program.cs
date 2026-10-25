using MiniAlarmBoard.Api.Storage;
using MiniAlarmBoard.Api.Services;
using MiniAlarmBoard.Api.Models;
using MiniAlarmBoard.Api.Hubs;
using MiniAlarmBoard.Api.Detection;
using MiniAlarmBoard.Api.Insights;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IAlarmStore, InMemoryAlarmStore>();
builder.Services.AddHostedService<AlarmGenerator>();
builder.Services.AddSignalR();
builder.Services.Configure<ZoneDetectorOptions>(builder.Configuration.GetSection(ZoneDetectorOptions.SectionName));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IZoneDetector, InMemoryZoneDetector>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<RuleBasedAlarmInsightService>();

if (string.IsNullOrWhiteSpace(builder.Configuration["Insights:ApiKey"]))
{
    builder.Services.AddSingleton<IAlarmInsightService>(sp => sp.GetRequiredService<RuleBasedAlarmInsightService>());
}
else
{
    builder.Services.AddSingleton<GeminiAlarmInsightService>();
    builder.Services.AddSingleton<IAlarmInsightService>(sp => new FallbackAlarmInsightService(
        sp.GetRequiredService<GeminiAlarmInsightService>(),
        sp.GetRequiredService<RuleBasedAlarmInsightService>(),
        sp.GetRequiredService<ILogger<FallbackAlarmInsightService>>()));
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.MapControllers();
app.UseAuthorization();

app.MapHub<AlarmHub>("/hubs/alarms");

app.Run();
