using Microsoft.Data.Sqlite;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace OrderApp.Services.Interfaces
{
    public interface IProductInOrdersRepository : IRepository<ProductsInOrders>
    {
       Task UpdateProductsInOrders(int quantity, int id, SqliteConnection? connection = null, SqliteTransaction? transaction = null);

       Task<int> GetQuantity(int id);

       Task InsertProductIntoProductsInOrder(int orderId, int productId, int quantity, SqliteConnection? connection = null, SqliteTransaction? transaction = null);

       Task DeleteProductInOrder(int orderId, int productId);
    }
}