
using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Insights;

public class RuleBasedAlarmInsightService : IAlarmInsightService
{
    private readonly ILogger<RuleBasedAlarmInsightService> _logger;
    private readonly TimeProvider _clock;

    public RuleBasedAlarmInsightService(ILogger<RuleBasedAlarmInsightService> logger, TimeProvider clock)
    {
        _logger = logger;
        _clock = clock;
    }

    public Task<AlarmInsight> ExplainAsync(ZoneAlarm zoneAlarm, IReadOnlyList<Alarm> recentAlarms, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var criticalCount = recentAlarms.Count(a => a.Priority == AlarmPriority.Critical);

        var windowSeconds = zoneAlarm.WindowSeconds;
        var summary = $"{zoneAlarm.AlarmCount} alarms in {zoneAlarm.Area} within {windowSeconds} seconds, {criticalCount} at Critical priority";

        var recommendation = criticalCount > 0 ? "triage the critical tags first" : "review the process conditions for this area";

        _logger.LogInformation("Generating insights for area {Area} with alarm count {Count}", zoneAlarm.Area, zoneAlarm.AlarmCount);


        return Task.FromResult(new AlarmInsight
        {
            Area = zoneAlarm.Area,
            Summary = summary,
            Recommendation = recommendation,
            Source = "rule-based",
            GeneratedAt = _clock.GetUtcNow()
        });
    }
}