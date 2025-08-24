using CommunityToolkit.Maui.Extensions;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.View;
using System.Collections.ObjectModel;

namespace OrderApp.Services
{

    public class PopupService
    {
        private readonly ClientServices _clientServices;
        private readonly ProductsServices _productServices;

        public PopupService(ClientServices clientServices, ProductsServices productServices)
        {
            _clientServices = clientServices;
            _productServices = productServices;
        }

        public async Task ShowAddClientPopupAsync(ObservableCollection<Client> clients)
        {
            var popup = new AddClientPopup(clients);
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }

        public async Task ShowAddProductPopupAsync(ObservableCollection<Product> products)
        {
            var popup = new AddProductPopup(products, null, 0);
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }

        public async Task ShowEditProductPopupAsync(ObservableCollection<Product> products, Product product)
        {
            var popup = new AddProductPopup(products, product, 1);
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }

        public async Task ShowAddOrderPopupAsync(ObservableCollection<Order> orders)
        {
            var clients = await _clientServices.GetClientsForPopup();
            var popup = new AddOrderPopup(clients, orders);
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }

        public async Task ShowAddEventPopupAsync()
        {
            var popup = new AddEventPopUp();
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }
        public async Task ShowEditEventPopupAsync(EventModel oldEvent)
        {
            var popup = new EditEventPopup( oldEvent);
            await Application.Current.Windows[0].Page.ShowPopupAsync(popup);
        }

    }
}
