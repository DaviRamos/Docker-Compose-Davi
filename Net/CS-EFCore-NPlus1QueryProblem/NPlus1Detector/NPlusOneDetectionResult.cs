namespace NPlus1Detector;

public class NPlusOneDetectionResult
{
    public string Query { get; init; } = string.Empty;
    public int ExecutionCount { get; init; }
    public double DurationMs { get; init; }
    public string? StackTrace { get; init; }
    public string? DbContextType { get; init; }
    public DateTime DetectedAt { get; init; } = DateTime.UtcNow;
}