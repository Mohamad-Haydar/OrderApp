using CommunityToolkit.Maui.Extensions;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.View;
using System.Collections.ObjectModel;

namespace OrderApp.Services
{

    public class PopupService
    {
        private readonly Page _mainPage;
        private readonly ClientServices _clientServices;
        private readonly ProductsServices _productServices;

        public PopupService(ClientServices clientServices, ProductsServices productServices)
        {
            _mainPage = Application.Current.MainPage;
            _clientServices = clientServices;
            _productServices = productServices;
        }

        public async Task ShowAddClientPopupAsync(ObservableCollection<Client> clients)
        {
            var popup = new AddClientPopup(clients);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddProductPopupAsync(ObservableCollection<Product> products)
        {
            var popup = new AddProductPopup(products);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddOrderPopupAsync(ObservableCollection<Order> orders)
        {
            var clients = await _clientServices.GetClientsForPopup();
            var popup = new AddOrderPopup(orders, clients);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddProductToOrderPopupAsync(Order order)
        {
            var products = await _productServices.GetProducts();
            var popup = new AddProductToOrderPopUp(products, order);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddEventPopupAsync(ObservableCollection<EventModel> EventsOfDay, DateOnly dateSelected)
        {
            var popup = new AddEventPopUp(EventsOfDay, dateSelected);
            await _mainPage.ShowPopupAsync(popup);
        }
        public async Task ShowEditEventPopupAsync(ObservableCollection<EventModel> EventsOfDay, DateOnly dateSelected, EventModel oldEvent)
        {
            var popup = new EditEventPopup(EventsOfDay, dateSelected, oldEvent);
            await _mainPage.ShowPopupAsync(popup);
        }

    }
}
