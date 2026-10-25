using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Insights;

public class FallbackAlarmInsightService : IAlarmInsightService
{
    private readonly IAlarmInsightService _primary;
    private readonly IAlarmInsightService _fallback;
    private readonly ILogger<FallbackAlarmInsightService> _logger;

    public FallbackAlarmInsightService(IAlarmInsightService primary, IAlarmInsightService fallback, ILogger<FallbackAlarmInsightService> logger)
    {
        _primary = primary;
        _fallback = fallback;
        _logger = logger;
    }

    public async Task<AlarmInsight> ExplainAsync(ZoneAlarm zoneAlarm, IReadOnlyList<Alarm> recentAlarms, CancellationToken cancellationToken)
    {
        try
        {
            return await _primary.ExplainAsync(zoneAlarm, recentAlarms, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Insight provider failed for {Area}, using the fallback", zoneAlarm.Area);
            return await _fallback.ExplainAsync(zoneAlarm, recentAlarms, cancellationToken);
        }
    }
}