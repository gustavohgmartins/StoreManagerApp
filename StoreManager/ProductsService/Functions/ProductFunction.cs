using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StoreManager.Core.Entities;
using StoreManager.Infrastructure.Data.Contexts;

namespace ProductFunctions.Functions
{
    public class ProductFunction
    {
        private readonly ApplicationDbContext _context;

        public ProductFunction(ApplicationDbContext context, ILogger<ProductFunction> logger)
        {
            _context = context;
        }

        [Function("GetProducts")]
        public async Task<IActionResult> GetProducts([HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req)
        {
            var products = await _context.Products.ToListAsync();

            return new OkObjectResult(products);
        }

        [Function("GetProductById")]
        public async Task<IActionResult> GetProductById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")] HttpRequest req, Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(product);
        }

        [Function("CreateProduct")]
        public async Task<IActionResult> CreateProduct([HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var product = JsonConvert.DeserializeObject<Product>(requestBody);

            var newProduct = new Product { Name = product.Name };

            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            return new CreatedResult($"/products/{newProduct.Id}", newProduct);
        }

        [Function("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([HttpTrigger(AuthorizationLevel.Function, "put", Route = "products")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateProduct = JsonConvert.DeserializeObject<Product>(requestBody);

            var product = await _context.Products.FindAsync(updateProduct.Id);
            if (product is null)
            {
                return new NotFoundResult();
            }

            product.Name = updateProduct.Name ?? product.Name;

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        [Function("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequest req, Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return new NotFoundResult();
            };

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
