using Microsoft.AspNetCore.SignalR;
using MiniAlarmBoard.Api.Storage;
using MiniAlarmBoard.Api.Models;
using MiniAlarmBoard.Api.Hubs;
using MiniAlarmBoard.Api.Detection;

namespace MiniAlarmBoard.Api.Services;

public class AlarmGenerator : BackgroundService
{
    private static readonly string[] IndustrialAreas = new string[]
    {
        "Sector 7 Foundry",
        "IronWorks District",
        "The Assembly Yards",
        "Chem-Lab Beta"
    };

    private readonly IAlarmStore _store;
    private readonly ILogger<AlarmGenerator> _logger;
    private readonly IHubContext<AlarmHub, IAlarmClient> _hub;
    private readonly IZoneDetector _detector;

    public AlarmGenerator(IAlarmStore store, IHubContext<AlarmHub, IAlarmClient> hub, ILogger<AlarmGenerator> logger, IZoneDetector zoneDetector)
    {
        _store = store;
        _logger = logger;
        _hub = hub;
        _detector = zoneDetector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var alarm = new Alarm
            {
                TagName = $"TAG-{Random.Shared.Next(1000, 9999)}",
                Description = "High Temperature trip",
                Area = IndustrialAreas[Random.Shared.Next(IndustrialAreas.Length)],
                Priority = AlarmPriority.High,
                Timestamp = DateTimeOffset.UtcNow
            };

            _store.Add(alarm);

            var zoneAlarm = _detector.OnAlarm(alarm);

            await _hub.Clients.Group(alarm.Area).AlarmRaised(alarm);

            if (zoneAlarm is not null)
            {
                await _hub.Clients.Group(zoneAlarm.Area).ZoneAlarmRaised(zoneAlarm);
            }

            _logger.LogInformation("Raised {Tag} in {Area}", alarm.TagName, alarm.Area);

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}
