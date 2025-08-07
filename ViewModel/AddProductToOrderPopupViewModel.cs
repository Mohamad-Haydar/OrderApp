using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class AddProductToOrderPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Product> products;


        [ObservableProperty]
        Product product;

        private readonly Popup _popup;
        private ProductsServices _productsServices;
        private OrderServices _orderServices;

        [ObservableProperty]
        DateTime today = DateTime.Today;

        [ObservableProperty]
        int quantity;

        Order _order;
        public AddProductToOrderPopupViewModel(ObservableCollection<Product> products, Order order, Popup popup, ProductsServices productsServices, OrderServices orderServices)
        {
            _popup = popup;
            product = new();
            _order = order;
            Products = products;
            _productsServices = productsServices;
            _orderServices = orderServices;
        }

        [RelayCommand]
        async Task Close() => await _popup.CloseAsync();

        [RelayCommand]
        async Task AddProductToOrderAsync()
        {
            //  if the user don't select a product, show error
            if (Product is null)
            {
                await Shell.Current.DisplayAlert("Error", "Please select a Product", "OK");
                return;
            }

            try
            {
                var availableStock = await _productsServices.GetStuckQuantity(Product.Id);

                // If not enough stock, stop here
                if (Quantity > availableStock)
                {
                    // throw exception
                    throw new Exception("Not enough product in stock");
                }

                await _orderServices.AddProductToOrder(_order, Product, Quantity);

                // close the popup
                await _popup.CloseAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            }
            finally
            {
            }
        }


    }
}
