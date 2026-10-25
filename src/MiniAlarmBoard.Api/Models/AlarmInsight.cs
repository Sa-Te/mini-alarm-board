using System.ComponentModel;

namespace MiniAlarmBoard.Api.Models;

public record AlarmInsight
{
    public required string Area { get; init; }
    public required string Summary { get; init; }
    public required string Recommendation { get; init; }
    public required string Source { get; init; }
    public required DateTimeOffset GeneratedAt { get; init; }
    public string? Model { get; init; }
    public int? PromptTokens { get; init; }
    public int? CompletionTokens { get; init; }
    public long? LatencyMs { get; init; }
}