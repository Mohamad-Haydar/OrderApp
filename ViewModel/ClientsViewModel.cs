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
            LoadClientsCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadClientsAsync()
        {
            try
            {
                IsBusy = true;

                var clientList = await _clientServices.GetClientsInfo();

                Clients.Clear();
                foreach (var client in clientList)
                    Clients.Add(client);
            }
            catch (Exception ex)
            {
                // TODO: handle/log error (maybe show a popup)
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        async Task AddClientAsync()
        {
            try
            {
                await _popupService.ShowAddClientPopupAsync(Clients);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while opening the add client popup. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToClientDetailsAsync()
        {
            try
            {
                // Implement navigation or details logic here if needed
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while navigating to client details. Please try again.", "OK");
            }
        }

        public async Task LoadClients()
        {
            try
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
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading clients. Please try again.", "OK");
            }
        }
    }
}
