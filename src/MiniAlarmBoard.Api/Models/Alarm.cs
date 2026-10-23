namespace MiniAlarmBoard.Api.Models;

public class Alarm
{
    public required string TagName { get; init; }
    public required string Description { get; init; }
    public required string Area { get; init; }
    public required AlarmPriority Priority { get; init; }
    public required DateTimeOffset Timestamp { get; init; }

}