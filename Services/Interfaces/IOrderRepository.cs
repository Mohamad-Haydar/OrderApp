using Microsoft.Data.Sqlite;
using OrderApp.Model;
using OrderApp.Services.Interfaces;
using System.Collections.ObjectModel;

namespace OrderApp.Services.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
       Task<List<Order>> GetOrders();
       Task UpdateOrderAsync(ObservableCollection<ProductsInOrders> productsInOrders, SqliteConnection? connection = null, SqliteTransaction? transaction = null);

       Task CreateOrder(int clientId, DateTime dateToPick);

       Task UpdateTotalAsync(float addedAmount, int orderId);
       Task SetTotalAsync(Order order, SqliteConnection? connection = null, SqliteTransaction? transaction = null);

       Task DeleteOrder(int orderId);
    }
}