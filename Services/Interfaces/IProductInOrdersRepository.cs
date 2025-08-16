using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IProductInOrdersRepository : IRepository<ProductsInOrders>
    {
        Task UpdateProductsInOrdersAsync(int quantity, int id);
        Task<int> GetQuantityAsync(int id);
        Task InsertProductIntoProductsInOrderAsync(int orderId, int productId, int quantity);
        Task DeleteProductInOrderAsync(int orderId, int productId);
    }
}