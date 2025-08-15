using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;
using OrderApp.Exceptions;

namespace OrderApp.ViewModel
{
    public partial class AddClientPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        Client client;

        private readonly ObservableCollection<Client> _clients;
        private ClientServices _clientServices;

        public AddClientPopupViewModel(ObservableCollection<Client> clients)
        {
            client = new();
            _clients = clients;
            _clientServices = ServiceHelper.Resolve<ClientServices>();
        }

        [RelayCommand]
        async Task AddClient()
        {
            try
            {
                await _clientServices.AddClient(Client.Name, Client.Details);
                _clients.Add(new Client() { Name = Client.Name, Details = Client.Details });
                Client = new Client();

                await Shell.Current.DisplayAlert("Success", "Client added successfully", "OK");
            }
            catch (ValidationException vex)
            {
                await Shell.Current.DisplayAlert("Validation Error", vex.Message, "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding the client. Please try again.", "OK");
            }
        }
    }
}
