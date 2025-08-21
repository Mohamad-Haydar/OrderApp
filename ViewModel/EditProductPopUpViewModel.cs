using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Diagnostics;

namespace OrderApp.ViewModel
{
    public partial class EditProductPopUpViewModel : ObservableObject
    {
        [ObservableProperty]
        Product eProduct;

        private ProductsServices _productsServices;
        public EditProductPopUpViewModel(Product oldProduct)
        {
            EProduct = oldProduct.Clone();
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
        }

        [RelayCommand]
        async Task UpdateProductAsync()
        {
            try
            {
                await _productsServices.ChangeProductInfo(EProduct);
            }
            catch (Exception ex)
            {
                // Example: show error to user
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");

                // Or log it
                Debug.WriteLine(ex);
            }
        }

    }
}
