using System.ComponentModel;
using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;
using ModelContextProtocol.Server;

namespace CoffeeShop.Tools;

[McpServerToolType]
public class ProductTool(AppDbContext context)
{
    [McpServerTool, Description("Get a list of available products.")]
    public async Task<List<ProductModel>> GetProductsAsync()
    {
        return await context.Products.AsNoTracking().Where(x => x.QuantityOnHand > 0).ToListAsync();
    }
}