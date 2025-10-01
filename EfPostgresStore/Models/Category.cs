using System;

namespace EfPostgresStore.Models;

public class Category
{
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Slug { get; set; } = String.Empty;

    public List<Product> Products { get; set; } = new List<Product>();
}

