namespace MiniAlarmBoard.Api.Detection;

public class ZoneDetectorOptions
{
    public const string SectionName = "ZoneDetector";

    public int WindowSeconds { get; set; } = 30;
    public int Threshold { get; set; } = 3;
}