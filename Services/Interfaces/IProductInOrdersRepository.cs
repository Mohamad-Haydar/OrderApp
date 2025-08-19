using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IProductInOrdersRepository : IRepository<ProductsInOrders>
    {
       Task UpdateProductsInOrders(int quantity, int id);

       Task<int> GetQuantity(int id);

       Task InsertProductIntoProductsInOrder(int orderId, int productId, int quantity);

       Task DeleteProductInOrder(int orderId, int productId);
    }
}