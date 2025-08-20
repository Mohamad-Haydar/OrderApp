using Microsoft.Data.Sqlite;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace OrderApp.Services.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProducts();

        Task<int> GetStuckQuantity(int productId);
        Task<ICollection<ProductsInOrders>> GetProductsInOrders(Order o);

        Task UpdateProductStock(int difference, int productId, SqliteConnection? connection = null, SqliteTransaction? transaction = null);

        Task AddProductAsync(string name, string description, float price, int quantity);

        Task UpdateProductImage(Product product);
    }
}