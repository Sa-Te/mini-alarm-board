namespace MiniAlarmBoard.Api.Models;


public class Alarm
{
    public required string TagName { get; set; }
    public required string Description { get; set; }
    public string Area { get; set; } = string.Empty;
    public AlarmPriority Priority { get; init; }
    public DateTimeOffset TimeStamp { get; set; }

}