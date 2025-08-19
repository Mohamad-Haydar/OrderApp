using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
       Task<List<Order>> GetOrders();
       Task UpdateOrderAsync(ObservableCollection<ProductsInOrders> productsInOrders);

       Task CreateOrder(int clientId, DateTime dateToPick);

       Task UpdateTotalAsync(float addedAmount, int orderId);
       Task SetTotalAsync(Order order);

       Task DeleteOrder(int orderId);
    }
}