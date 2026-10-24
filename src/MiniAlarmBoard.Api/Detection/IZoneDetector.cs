using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Detection;

public interface IZoneDetector
{
    ZoneAlarm? OnAlarm(Alarm alarm);
}