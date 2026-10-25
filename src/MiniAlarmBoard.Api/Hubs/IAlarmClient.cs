using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Hubs;

public interface IAlarmClient
{
    Task AlarmRaised(Alarm alarm);
    Task ZoneAlarmRaised(ZoneAlarm zoneAlarm);
    Task InsightReady(AlarmInsight insight);
}
