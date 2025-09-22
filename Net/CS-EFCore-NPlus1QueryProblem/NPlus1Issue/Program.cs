using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedParameter.Local
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedVariable

namespace NPlus1Issue;

/*
    What is the N+1 Query Problem?

    - 1 query to get a list of items
    - N queries to get related data for each item (where N = number of items)

    Why Is This Bad?

    - Performance: Each database round-trip takes time
    - Resource usage: Database gets overwhelmed with requests
    - Scalability: Gets worse as your data grows (1000 blogs = 1001 queries!)
 */

// Master or Parent (1)
public class Blog
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public List<Post> Posts { get; init; } = new();
}

// Detail or Child (N)
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=n1demo.db");
        }

        optionsBuilder.EnableSensitiveDataLogging()
            .LogTo(message =>
            {
                if (message.Contains("SELECT"))
                {
                    Console.WriteLine(message);
                }
            }, LogLevel.Information);
    }
}

static class Program
{
    static async Task Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=n1demo.db")
            .Options;

        await using var context = new AppDbContext(options);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await SeedDataAsync(context);

        // N+1 Problem
        Console.WriteLine("(ISSUE) Executing N+1 query pattern (loading blogs then posts individually)...");

        var stopwatch1 = Stopwatch.StartNew();

        // 1 query gets all blogs (1 query to get a list)
        var blogs = await context.Blogs.ToListAsync();

        // 10 more queries get posts for each blog (one per blog) - (N queries for each list's item)
        foreach (var blog in blogs)
        {
            // This is the N+1 query pattern
            var posts = await context.Posts
                .Where(p => p.BlogId == blog.Id)
                .ToListAsync();
        }
        // Total: 11 queries instead of just 1!

        stopwatch1.Stop();

        Console.WriteLine(
            $"Loaded {blogs.Count} blogs plus loading posts for each blog in {stopwatch1.ElapsedMilliseconds}ms.");
        Console.WriteLine();

        
        // Solution 1
        Console.WriteLine("(SOLUTION 1) Executing CORRECT approach (single query with Include)...");

        var stopwatch2 = Stopwatch.StartNew();
        var blogsWithPosts = await context.Blogs
            .Include(b => b.Posts)
            .ToListAsync();
        stopwatch2.Stop();

        Console.WriteLine($"Loaded {blogsWithPosts.Count} blogs with all posts in {stopwatch2.ElapsedMilliseconds}ms.");
        Console.WriteLine();
        
        
        // Solution 2
        Console.WriteLine("(SOLUTION 2) Executing CORRECT approach (projection with anonymous types or DTOs)...");

        var stopwatch3 = Stopwatch.StartNew();
        var blogsData = await context.Blogs
            .Select(b => new
            {
                b.Id,
                b.Title,
                Posts = b.Posts.Select(p => new { p.Id, p.Content }).ToList()
            })
            .ToListAsync();
        stopwatch3.Stop();

        Console.WriteLine($"Loaded {blogsData.Count} blogs with all posts in {stopwatch3.ElapsedMilliseconds}ms.");
        Console.WriteLine();
        
    }

    private static async Task SeedDataAsync(AppDbContext context)
    {
        // 10 blogs (Blog records 1 through 10)
        // 30 posts (3 posts for each of the 10 blogs)
        // Total: 40 records in the database

        Console.WriteLine("Seeding database with test data...");

        var blogs = Enumerable.Range(1, 10).Select(i => new Blog
        {
            Title = $"Blog {i}",
            Posts = Enumerable.Range(1, 3).Select(j => new Post
            {
                Content = $"Post {j} content for Blog {i}"
            }).ToList()
        }).ToList();

        context.Blogs.AddRange(blogs);
        await context.SaveChangesAsync();

        Console.WriteLine($"Created {blogs.Count} blogs with {blogs.Sum(b => b.Posts.Count)} posts total.\n");
    }
}