namespace Poststore.Models;

public class Product
{
    public int Id { get; set; }
    public Heading Heading { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public string DefaultLanguage { get; set; } = "en-us";

    public List<Translation> Translations { get; set; } = [];
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}