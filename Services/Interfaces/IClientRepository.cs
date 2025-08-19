using OrderApp.Model;
using System.Collections.ObjectModel;
using OrderApp.Services.Interfaces;

namespace OrderApp.Services.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<ObservableCollection<Client>> GetClientsForPopup();

        Task<List<Client>> GetClientsInfo();

        Task<string> GetCustomer(Order order);

        Task AddClient(string name, string details);
    }
}