using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using MiniAlarmBoard.Api.Detection;
using MiniAlarmBoard.Api.Models;

namespace MiniAlarmBoard.Tests;

public class InMemoryZoneDetectorTests
{
    [Fact]
    public void RaisesOneZoneAlarmWhenTheThresholdIsCrossed()
    {
        var clock = new FakeTimeProvider();
        var detector = CreateDetector(clock, windowSeconds: 30, threshold: 3);

        Assert.Null(detector.OnAlarm(NewAlarm("Sector 7 Foundry")));
        Assert.Null(detector.OnAlarm(NewAlarm("Sector 7 Foundry")));

        var zoneAlarm = detector.OnAlarm(NewAlarm("Sector 7 Foundry"));

        Assert.NotNull(zoneAlarm);
        Assert.Equal("Sector 7 Foundry", zoneAlarm!.Area);
        Assert.Equal(3, zoneAlarm.AlarmCount);
        Assert.Equal(30, zoneAlarm.WindowSeconds);
    }

    [Fact]
    public void StaysQuietWhileTheAreaIsStillOverTheThreshold()
    {
        var clock = new FakeTimeProvider();
        var detector = CreateDetector(clock, windowSeconds: 30, threshold: 3);

        detector.OnAlarm(NewAlarm("Sector 7 Foundry"));
        detector.OnAlarm(NewAlarm("Sector 7 Foundry"));
        Assert.NotNull(detector.OnAlarm(NewAlarm("Sector 7 Foundry")));

        Assert.Null(detector.OnAlarm(NewAlarm("Sector 7 Foundry")));
        Assert.Null(detector.OnAlarm(NewAlarm("Sector 7 Foundry")));
    }

    [Fact]
    public void CountsEachAreaSeparately()
    {
        var clock = new FakeTimeProvider();
        var detector = CreateDetector(clock, windowSeconds: 30, threshold: 3);

        detector.OnAlarm(NewAlarm("Sector 7 Foundry"));
        detector.OnAlarm(NewAlarm("Chem-Lab Beta"));
        detector.OnAlarm(NewAlarm("Sector 7 Foundry"));
        Assert.Null(detector.OnAlarm(NewAlarm("Chem-Lab Beta")));

        var zoneAlarm = detector.OnAlarm(NewAlarm("Sector 7 Foundry"));

        Assert.NotNull(zoneAlarm);
        Assert.Equal("Sector 7 Foundry", zoneAlarm!.Area);
    }

    [Fact]
    public void RaisesAgainAfterTheWindowRollsAndTheAreaRecovers()
    {
        var clock = new FakeTimeProvider();
        var detector = CreateDetector(clock, windowSeconds: 30, threshold: 3);

        detector.OnAlarm(NewAlarm("The Assembly Yards"));
        detector.OnAlarm(NewAlarm("The Assembly Yards"));
        Assert.NotNull(detector.OnAlarm(NewAlarm("The Assembly Yards")));

        clock.Advance(TimeSpan.FromSeconds(31));

        Assert.Null(detector.OnAlarm(NewAlarm("The Assembly Yards")));
        Assert.Null(detector.OnAlarm(NewAlarm("The Assembly Yards")));
        Assert.NotNull(detector.OnAlarm(NewAlarm("The Assembly Yards")));
    }

    private static InMemoryZoneDetector CreateDetector(TimeProvider clock, int windowSeconds, int threshold)
        => new(
            Options.Create(new ZoneDetectorOptions { WindowSeconds = windowSeconds, Threshold = threshold }),
            clock,
            NullLogger<InMemoryZoneDetector>.Instance);

    private static Alarm NewAlarm(string area) => new()
    {
        TagName = "TAG-0001",
        Description = "Test alarm",
        Area = area,
        Priority = AlarmPriority.High,
        Timestamp = DateTimeOffset.UnixEpoch
    };
}