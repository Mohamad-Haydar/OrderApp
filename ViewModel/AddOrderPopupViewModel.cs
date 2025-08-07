using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.View;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class AddOrderPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        Order order;

        [ObservableProperty]
        Client client;

        [ObservableProperty]
        ObservableCollection<Client> clients;

        [ObservableProperty]
        DateTime today = DateTime.Today;

        public ObservableCollection<Order> _orders;
        private readonly Popup _popup;
        private OrderServices _orderServices;

        public AddOrderPopupViewModel(ObservableCollection<Order> orders, ObservableCollection<Client> clients, Popup popup, OrderServices orderServices)
        {
            _popup = popup;
            order = new();
            _orders = orders;
            Clients = clients;
            _orderServices = orderServices;
        }

        [RelayCommand]
        async Task Close() => await _popup.CloseAsync();

        [RelayCommand]
        async Task AddOrder()
        {
            // if client is not selected show an error
            if (Client is null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a client", "OK");
                return;
            }
            var connection = AdoDatabaseService.GetConnection();
            try
            {
                await _orderServices.CreateOrder(Client.Id, Order.DateToPick);

                _orders.Add(new Order()
                {
                    ClientId = Client.Id,
                    Total = 0,
                    DateToPick = Order.DateToPick
                });
                // close the popup after finish the work
                await _popup.CloseAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            }
            finally
            {
                connection.Close();
            }
        }


    }
}
