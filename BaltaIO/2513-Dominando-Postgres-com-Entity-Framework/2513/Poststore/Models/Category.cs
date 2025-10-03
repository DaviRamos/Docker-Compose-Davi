namespace Poststore.Models;

public class Category
{
    public int Id { get; set; }
    public Heading Heading { get; set; } = new();

    public List<Product> Products { get; set; } = [];
}