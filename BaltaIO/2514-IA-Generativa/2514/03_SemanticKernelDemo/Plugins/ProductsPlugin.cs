using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelDemo.Plugins;

public class ProductsPlugin
{
    private readonly List<Product> _products =
    [
        new(1, "Mousepad", true, 10),
        new(2, "Mouse Gamer", true, 8),
        new(3, "Teclado Gamer", true, 1),
        new(4, "Capa Monitor", false, 1),
        new(5, "Monitor Gamer", true, 5),
    ];

    [KernelFunction("get_products")]
    [Description("Gets a list of products and their current state")]
    public async Task<List<Product>> GetProductsAsync()
    {
        await Task.Delay(1);
        return _products;
    }

    [KernelFunction("get_state")]
    [Description("Gets the state of a particular product")]
    public async Task<Product?> GetStateAsync([Description("The ID of the product")] int id)
    {
        await Task.Delay(1);
        return _products.FirstOrDefault(light => light.Id == id);
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the product")]
    public async Task<Product?> ChangeStateAsync(int id, Product model)
    {
        await Task.Delay(1);
        var product = _products.FirstOrDefault(light => light.Id == id);

        if (product == null)
            return null;

        product = new Product(model.Id, model.Title, model.IsActive, model.QuantityOnHand);

        return product;
    }
}

public record Product(int Id, string Title, bool IsActive, int QuantityOnHand);