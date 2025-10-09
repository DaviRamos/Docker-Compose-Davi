using System.ComponentModel;
using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

namespace CoffeeShop.Plugins;

public class ProductPlugin(AppDbContext context)
{
    [KernelFunction("get_products")]
    [Description("Gets a list of products and their states")]
    public async Task<List<ProductModel>> GetProductsAsync()
    {
        return await context.Products.AsNoTracking().ToListAsync();
    }

    [KernelFunction("get_state")]
    [Description("Get the state of the product")]
    public async Task<ProductModel?> GetProductStateAsync([Description("The ID of the product")] int id)
    {
        return await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    [KernelFunction("get_zipcode_by-address")]
    [Description("Calculates the shipping price of the product")]
    public async Task<Dictionary<string, string>> GetCustomerZipCodeAsync(
        [Description("Customer address")] string address)
    {
        await Task.Delay(1);
        var addresses = new Dictionary<string, string>
        {
            { "77002", "173, Main Street, Houston, TX" },
            { "10001", "350, 7th Avenue, New York, NY" },
            { "90001", "742, Sunset Boulevard, Los Angeles, CA" },
            { "60601", "200, Michigan Avenue, Chicago, IL" },
            { "94102", "500, Market Street, San Francisco, CA" },
            { "30301", "123, Peachtree Street, Atlanta, GA" },
            { "75201", "400, Elm Street, Dallas, TX" },
            { "98101", "789, Pine Street, Seattle, WA" },
            { "33101", "250, Ocean Drive, Miami, FL" },
            { "85001", "120, Camelback Road, Phoenix, AZ" },
            { "48201", "95, Woodward Avenue, Detroit, MI" }
        };

        return addresses;
    }

    [KernelFunction("get_shipping_price")]
    [Description("Calculates the shipping price of the product")]
    public async Task<decimal?> CalculateShippingPriceAsync(
        [Description("The zipcode of the addres the product will be shipped")] string zipCode)
    {
        await Task.Delay(1);
        var random = new Random();
        var randomDouble = random.NextDouble();
        var result = 5 + (decimal)(randomDouble * (54 - 5));
        return result;
    }
}