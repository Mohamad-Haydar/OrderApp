using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<ObservableCollection<Product>> GetProductsAsync();
        Task<int> GetStockQuantityAsync(int productId);
        Task<float> GetProductsInOrdersAsync(ObservableCollection<ProductsInOrders> products, Order o);
        Task UpdateProductStockAsync(int difference, int productId);
        Task AddProductAsync(string name, string description, float price, int quantity);
        Task UpdateProductImageAsync(Product product);
    }
}