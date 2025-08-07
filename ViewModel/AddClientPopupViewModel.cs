using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class AddClientPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        Client client;

        private readonly ObservableCollection<Client> _clients;
        private readonly Popup _popup;
        private ClientServices _clientServices;

        public AddClientPopupViewModel(ObservableCollection<Client> clients, Popup popup, ClientServices clientServices)
        {
            _popup = popup;
            client = new();
            _clients = clients;
            _clientServices = clientServices;
        }

        [RelayCommand]
        async Task AddClient()
        {
            try
            {
                await _clientServices.AddClient(Client.Name, Client.Details);
                _clients.Add(new Client() { Name = Client.Name, Details = Client.Details });
                Client = new Client();

                await _popup.CloseAsync();

                await Shell.Current.DisplayAlert("Success", "Client added successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Database error: " + ex.Message, "OK");
            }
        }
    }
}
