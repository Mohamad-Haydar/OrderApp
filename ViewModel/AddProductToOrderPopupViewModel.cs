using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
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

        private ProductsServices _productsServices;
        private OrderServices _orderServices;

        [ObservableProperty]
        DateTime today = DateTime.Today;

        [ObservableProperty]
        int quantity;

        Order _order;
        public AddProductToOrderPopupViewModel(ObservableCollection<Product> products, Order order)
        {
            product = new();
            _order = order;
            Products = products;
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
        }

        
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
                    await Shell.Current.DisplayAlert("Success", "The product is updated successfully", "Ok");
                }
                else
                {
                    await _orderServices.AddProductToOrder(_order, Product, Quantity);
                }
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
