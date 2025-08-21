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
            var popup = new AddProductPopup(products, null, 0);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowEditProductPopupAsync(ObservableCollection<Product> products, Product product)
        {
            var popup = new AddProductPopup(products, product, 1);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddOrderPopupAsync()
        {
            var clients = await _clientServices.GetClientsForPopup();
            var popup = new AddOrderPopup(clients);
            await _mainPage.ShowPopupAsync(popup);
        }

        public async Task ShowAddEventPopupAsync()
        {
            var popup = new AddEventPopUp();
            await _mainPage.ShowPopupAsync(popup);
        }
        public async Task ShowEditEventPopupAsync(EventModel oldEvent)
        {
            var popup = new EditEventPopup( oldEvent);
            await _mainPage.ShowPopupAsync(popup);
        }

    }
}
