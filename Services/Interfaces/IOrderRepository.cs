using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task UpdateOrderProductsAsync(ObservableCollection<ProductsInOrders> productsInOrders);
        Task CreateOrderAsync(int clientId, DateTime dateToPick, int userId);
        Task UpdateTotalAsync(float addedAmount, int orderId);
        Task SetTotalAsync(Order order);
    }
}