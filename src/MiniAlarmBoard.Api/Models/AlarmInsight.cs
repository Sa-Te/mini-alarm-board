namespace MiniAlarmBoard.Api.Models;

public record AlarmInsight
{
    public required string Area { get; init; }
    public required string Summary { get; init; }
    public required string Recommendation { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }
}