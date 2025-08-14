using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class AddProductPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        Product product;

        private readonly ObservableCollection<Product> _products;
        private ProductsServices _productsServices;

        public AddProductPopupViewModel(ObservableCollection<Product> products)
        {
            product = new();
            _products = products;
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
        }
        [RelayCommand]
        async Task AddProduct()
        {
            try
            {
                await _productsServices.AddProductAsync(Product.Name, Product.Description, Product.Price, Product.Quantity);
                _products.Add(new Product() { Name = Product.Name, Description = Product.Description, Price = Product.Price, Quantity = Product.Quantity});
                Product = new Product();

                await Shell.Current.DisplayAlert("Success", "Product added successfully", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "error: " + ex.Message, "OK");
            }
        }
    }
}
