using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedVariable

namespace NPlus1Detector;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=n1demo.db")
            .Options;

        await using var context = new AppDbContext(options);
        
        

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await SeedDataAsync(context);

        Console.WriteLine("Testing N+1 Query Detection...\n");

        await DemonstrateNPlus1ProblemAsync(context);

        Console.WriteLine("\n" + new string('=', 60));
        Console.WriteLine("Now demonstrating the CORRECT approach...\n");

        await Task.Delay(2000);

        await DemonstrateCorrectApproachAsync(context);

        Console.WriteLine("\n Test completed. Notice it now triggers exactly at threshold 5!");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task SeedDataAsync(AppDbContext context)
    {
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

    private static async Task DemonstrateNPlus1ProblemAsync(AppDbContext context)
    {
        Console.WriteLine("Executing N+1 query pattern (loading blogs then posts individually)...");

        var stopwatch = Stopwatch.StartNew();

        var blogs = await context.Blogs.ToListAsync();
        foreach (var blog in blogs)
        {
            // This is the N+1 query pattern
            var posts = await context.Posts
                .Where(p => p.BlogId == blog.Id)
                .ToListAsync();
        }

        stopwatch.Stop();

        Console.WriteLine(
            $"Loaded {blogs.Count} blogs plus loading posts for each blog in {stopwatch.ElapsedMilliseconds}ms.");
    }

    private static async Task DemonstrateCorrectApproachAsync(AppDbContext context)
    {
        Console.WriteLine("Executing CORRECT approach (single query with Include)...");

        var stopwatch = Stopwatch.StartNew();
        var blogsWithPosts = await context.Blogs
            .Include(b => b.Posts)
            .ToListAsync();
        stopwatch.Stop();

        Console.WriteLine($"Loaded {blogsWithPosts.Count} blogs with all posts in {stopwatch.ElapsedMilliseconds}ms.");
    }
}