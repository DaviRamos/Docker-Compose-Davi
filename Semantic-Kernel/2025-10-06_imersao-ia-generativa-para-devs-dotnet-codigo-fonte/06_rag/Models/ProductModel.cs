using Microsoft.Extensions.VectorData;

namespace CoffeeShop.Models;

public class ProductModel
{
    [VectorStoreKey] public int Id { get; set; }

    [VectorStoreData(StorageName = "Title")]
    public string Title { get; set; } = string.Empty;

    [VectorStoreData(StorageName = "Category")]
    public string Category { get; set; } = string.Empty;

    // [VectorStoreData(StorageName = "Description")]
    public string Description { get; set; } = string.Empty;
    
    public float Price { get; set; }
    public int QuantityOnHand { get; set; }

    [VectorStoreVector(1024)] public string Embeddings => this.ToString();

    public override string ToString() =>
        $"Category: {Category}, " +
        $"Title: {Title}, "
        .Trim();
}
/*
CREATE TABLE Products (
    ...
);

CREATE VIRTUAL TABLE vec_Products_XXXXX (
    Title FLOAT[XXX] distance_metric=cosine
);
*/