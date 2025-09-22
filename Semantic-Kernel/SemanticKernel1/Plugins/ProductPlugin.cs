using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernel1.Plugins;

public class ProductPlugin
{
    private readonly List<Product> _products =
    [
        new(1,"Mousepad", true, 10),
        new(1,"Mouse Gamer", true, 8),
        new(1,"Teclado Gamer", true, 1),
        new(1,"Capa Monitor", false, 11),
        new(1,"Moniotr Gamer", true, 5)
    ];

    [KernelFunction("get_products")]
    [Description("Return list of products")]
    public async Task<List<Product>> GetProductsAsync()
    {
        await Task.Delay(1);

        return _products;
    }
}

public record Product(int Id, string Title, bool IsActive, int QuantityOnHand);