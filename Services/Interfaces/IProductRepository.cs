using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProducts();

        Task<int> GetStuckQuantity(int productId);
        Task<ICollection<ProductsInOrders>> GetProductsInOrders(Order o);

        Task UpdateProductStock(int difference, int productId);

        Task AddProductAsync(string name, string description, float price, int quantity);

        Task UpdateProductImage(Product product);
    }
}