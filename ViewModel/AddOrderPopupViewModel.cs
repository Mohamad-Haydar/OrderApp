using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.View;
using System.Collections.ObjectModel;
using OrderApp.Exceptions;

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

        public AddOrderPopupViewModel(ObservableCollection<Client> clients)
        {
            order = new();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
            Clients = clients;
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
                if (Order.DateToPick < DateTime.Today)
                    throw new ValidationException("Order date cannot be in the past.");
                await _orderServices.CreateOrder(Client.Id, Order.DateToPick);
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
