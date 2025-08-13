using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class ClientsViewModel : BaseViewModel
    {
        private readonly PopupService _popupService;
        private readonly ClientServices _clientServices;

        public ObservableCollection<Client> Clients { get; }

        public ClientsViewModel()
        {
            Clients = [];
            _popupService = ServiceHelper.Resolve<PopupService>();
            _clientServices = ServiceHelper.Resolve<ClientServices>();
        }


        [RelayCommand]
        async Task AddClientAsync()
        {
            await _popupService.ShowAddClientPopupAsync(Clients);
        }

        [RelayCommand]
        async Task GoToClientDetailsAsync()
        {

        }

        public async Task LoadClients()
        {
            Clients.Clear();
            var result = await _clientServices.GetClientsInfo();
            foreach (var item in result) 
            {
                Clients.Add(new Client
                {
                    Id = item.Id,
                    Name = item.Name,
                    Details = item.Details
                });
            }

        }
    }
}
