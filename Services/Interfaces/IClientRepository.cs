using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<ObservableCollection<Client>> GetClientsForPopupAsync();
        Task<List<Client>> GetClientsInfoAsync();
        Task<string> GetCustomerAsync(Order order);
    }
}