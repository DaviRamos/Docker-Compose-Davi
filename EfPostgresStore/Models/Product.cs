using System;

namespace EfPostgresStore.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Slug { get; set; } = String.Empty;
    public DateTime CreatedAtUTc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUTc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public string DefaultLanguage { get; set; } = "en-us";

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
