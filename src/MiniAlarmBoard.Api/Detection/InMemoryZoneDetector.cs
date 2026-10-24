using Microsoft.Extensions.Options;
using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Detection;

public class InMemoryZoneDetector : IZoneDetector
{
    private readonly object _gate = new();
    private readonly Dictionary<string, List<DateTimeOffset>> _recentAlarms = new();
    private readonly HashSet<string> _overThresholdAreas = new();
    private readonly IOptions<ZoneDetectorOptions> _options;
    private readonly TimeProvider _clock;
    private readonly ILogger<InMemoryZoneDetector> _logger;

    public InMemoryZoneDetector(IOptions<ZoneDetectorOptions> options, TimeProvider clock, ILogger<InMemoryZoneDetector> logger)
    {
        _options = options;
        _clock = clock;
        _logger = logger;
    }

    public ZoneAlarm? OnAlarm(Alarm alarm)
    {
        var options = _options.Value;
        var window = TimeSpan.FromSeconds(options.WindowSeconds);

        var threshold = options.Threshold;
        var now = _clock.GetUtcNow();

        lock (_gate)
        {
            if (!_recentAlarms.TryGetValue(alarm.Area, out var alarmTimes))
            {
                alarmTimes = new List<DateTimeOffset>();
                _recentAlarms[alarm.Area] = alarmTimes;
            }

            alarmTimes.Add(now);
            var cutoff = now - window;
            alarmTimes.RemoveAll(t => t < cutoff);

            bool isMarked = _overThresholdAreas.Contains(alarm.Area);

            if (alarmTimes.Count >= threshold && !isMarked)
            {
                _overThresholdAreas.Add(alarm.Area);
                var windowStart = alarmTimes[0];
                _logger.LogWarning("Zone {Area} exceeded threshold with {Count} alarms.", alarm.Area, alarmTimes.Count);

                return new ZoneAlarm
                {
                    Area = alarm.Area,
                    AlarmCount = alarmTimes.Count,
                    WindowSeconds = options.WindowSeconds,
                    WindowStart = windowStart,
                    RaisedAt = now
                };
            }

            if (alarmTimes.Count < threshold && isMarked)
            {
                _overThresholdAreas.Remove(alarm.Area);
                _logger.LogInformation("Zone {Area} recovered", alarm.Area);
            }


            return null;

        }
    }
}