using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Insights;

public interface IAlarmInsightService
{
    Task<AlarmInsight> ExplainAsync(ZoneAlarm zoneAlarm, IReadOnlyList<Alarm> recentAlarms, CancellationToken cancellationToken);
}