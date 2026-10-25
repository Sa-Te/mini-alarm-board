using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Api.Insights;

public class GeminiAlarmInsightService : IAlarmInsightService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<GeminiAlarmInsightService> _logger;
    private readonly TimeProvider _clock;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAlarmInsightService(
        IHttpClientFactory httpFactory,
        IConfiguration configuration,
        TimeProvider clock, ILogger<GeminiAlarmInsightService> logger
    )
    {
        _httpFactory = httpFactory;
        _logger = logger;
        _clock = clock;
        _apiKey = configuration["Insights:ApiKey"] ?? throw new InvalidOperationException("Insights:ApiKey is not configured");
        _model = configuration["Insights:Model"] ?? "gemini-2.5-flash";
    }

    public async Task<AlarmInsight> ExplainAsync(ZoneAlarm zoneAlarm, IReadOnlyList<Alarm> recentAlarms, CancellationToken cancellationToken)
    {
        var started = Stopwatch.GetTimestamp();

        var tags = string.Join(", ", recentAlarms.Select(a => $"{a.TagName} ({a.Priority})"));

        var prompt =
           "An industrial alarm area has just crossed its alarm flood threshold. " +
           $"Area: {zoneAlarm.Area}. " +
           $"Alarms in the last {zoneAlarm.WindowSeconds} seconds: {zoneAlarm.AlarmCount}. " +
           $"Tags: {tags}. " +
           "Reply with JSON only, no markdown, in exactly this shape: " +
           "{\"summary\": \"one sentence\", \"recommendation\": \"one sentence\"}";

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { responseMimeType = "application/json" }
        };

        using var client = _httpFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent")
        {
            Content = JsonContent.Create(requestBody)
        };
        request.Headers.Add("x-goog-api-key", _apiKey);

        using var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Gemini failed with {Status}: {Error}", (int)response.StatusCode, error);
            throw new HttpRequestException($"Gemini returned {(int)response.StatusCode}.");
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var latencyMs = (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds;

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        var raw = root.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? throw new InvalidOperationException("The model returned no text");

        var text = raw.Trim();
        if (text.StartsWith("```"))
        {
            text = text[(text.IndexOf("\n") + 1)..text.LastIndexOf("```", StringComparison.Ordinal)].Trim();

        }

        string? summary;
        string? recommendation;

        try
        {
            using var payload = JsonDocument.Parse(text);
            summary = payload.RootElement.TryGetProperty("summary", out var s) ? s.GetString() : null;
            recommendation = payload.RootElement.TryGetProperty("recommendation", out var r) ? r.GetString() : null;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning("Gemini returned unparseable JSON: {Raw}", raw);
            throw new InvalidOperationException("The model did not return valid JSON.", ex);
        }

        if (string.IsNullOrWhiteSpace(summary) || summary.Length < 20 || summary.Length > 500)
        {
            _logger.LogWarning("Summary failed validation: {Summary}", summary);
            throw new InvalidOperationException("The insight summary failed validation.");
        }

        if (string.IsNullOrWhiteSpace(recommendation))
        {
            throw new InvalidOperationException("The insight recommendation failed validation.");
        }

        int? promptTokens = null;
        int? completionTokens = null;

        if (root.TryGetProperty("usageMetadata", out var usage))
        {
            if (usage.TryGetProperty("promptTokenCount", out var p)) promptTokens = p.GetInt32();
            if (usage.TryGetProperty("candidatesTokenCount", out var c)) completionTokens = c.GetInt32();
        }

        _logger.LogInformation(
            "Gemini insight for {Area}: {Latency} ms, {PromptTokens} prompt tokens, {CompletionTokens} completion tokens",
            zoneAlarm.Area, latencyMs, promptTokens, completionTokens);

        return new AlarmInsight
        {
            Area = zoneAlarm.Area,
            Summary = summary,
            Recommendation = recommendation,
            Source = $"gemini:{_model}",
            GeneratedAt = _clock.GetUtcNow(),
            Model = _model,
            PromptTokens = promptTokens,
            CompletionTokens = completionTokens,
            LatencyMs = latencyMs
        };
    }
}