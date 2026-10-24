namespace MiniAlarmBoard.Api.Models;

public record ZoneAlarm
{
    public required string Area { get; init; }
    public required int AlarmCount { get; init; }
    public required int WindowSeconds { get; init; }
    public required DateTimeOffset WindowStart { get; init; }
    public required DateTimeOffset RaisedAt { get; init; }
}