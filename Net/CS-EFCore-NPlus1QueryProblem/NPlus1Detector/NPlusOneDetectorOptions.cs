namespace NPlus1Detector;

public class NPlusOneDetectorOptions
{
    public int Threshold { get; init; } = 3;
    public int DetectionWindowMs { get; init; } = 2000;
    public int CooldownMs { get; init; } = 3000;
    public bool CaptureStackTrace { get; init; } = true;
    public bool LogToConsole { get; init; } = true;
    public int CleanupIntervalMinutes { get; init; } = 5;
    public Action<NPlusOneDetectionResult>? OnDetection { get; init; }
}