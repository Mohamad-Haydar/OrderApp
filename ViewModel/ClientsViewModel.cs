using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public ClientsViewModel(PopupService popupService, LocalizationService localizationService, ThemeService themeService, ClientServices clientServices) : base(localizationService, themeService)
        {
            Clients = [];
            _popupService = popupService;
            _clientServices = clientServices;
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
