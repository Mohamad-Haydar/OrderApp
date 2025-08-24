using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Exceptions;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
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

        private OrderServices _orderServices;
        ObservableCollection<Order> _ordersInPage;

        public AddOrderPopupViewModel(ObservableCollection<Client> clients, ObservableCollection<Order> orders)
        {
            order = new();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
            Clients = clients;
            _ordersInPage = orders;
        }

       
        [RelayCommand]
        async Task AddOrder()
        {
            // if client is not selected show an error
            if (Client is null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a client", "OK");
                return;
            }
            try
            {
                // Add validation for date
                if (!Order.IsValid(out var error))
                    throw new ValidationException(error);
                var orderCreated = await _orderServices.CreateOrder(Client.Id, Order.DateToPick);
                _ordersInPage.Add(orderCreated);
                // Signal to close the popup
                WeakReferenceMessenger.Default.Send(new ClosePopupMessage());
            }
            catch (ValidationException vex)
            {
                await Shell.Current.DisplayAlert("Validation Error", vex.Message, "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while creating the order. Please try again.", "OK");
            }
        }


    }
}
