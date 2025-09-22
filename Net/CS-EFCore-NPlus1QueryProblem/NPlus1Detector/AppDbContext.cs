using Microsoft.EntityFrameworkCore;

namespace NPlus1Detector;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedVariable
public class Blog
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public List<Post> Posts { get; init; } = new();
}

public class Post
{
    public int Id { get; init; }
    public string Content { get; init; } = "";
    public int BlogId { get; init; }
    public Blog Blog { get; init; } = null!;
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnConfiguring(DbContextOptionsBuilder
        optionsBuilder)
    {
        optionsBuilder.AddNPlusOneDetector(new NPlusOneDetectorOptions()
        {
            CaptureStackTrace = true,
            LogToConsole = true,
            Threshold = 5,
            DetectionWindowMs = 2000,
            CooldownMs = 3000,
            CleanupIntervalMinutes = 5,
            OnDetection = (result) =>
            {
                Console.WriteLine($"N+1 DETECTED: {result.ExecutionCount} queries in {result.DurationMs:F2}ms");
                Console.WriteLine($"Location: {result.StackTrace?.Split('\n').FirstOrDefault()?.Trim()}");
                Console.WriteLine($"Query: {(result.Query.Length > 80 ? result.Query.Substring(0, 80) + "..." : result.Query)}");
                Console.WriteLine($"Time: {result.DetectedAt:HH:mm:ss}");
                Console.WriteLine($"Context: {result.DbContextType}");
                Console.WriteLine(new string('-', 50));
            }
        });
    }
}