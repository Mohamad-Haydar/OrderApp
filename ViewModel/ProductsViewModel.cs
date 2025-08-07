using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class ProductsViewModel : BaseViewModel
    {
        private readonly PopupService _popupService;
        private readonly ProductsServices _productsServices;

        public ObservableCollection<Product> Products { get;  }

        public ProductsViewModel(PopupService popupService, LocalizationService localizationService, ThemeService themeService, ProductsServices productsServices) : base(localizationService, themeService)
        {
            Products = [];
            _popupService = popupService;
            _productsServices = productsServices;
        }

        [RelayCommand]
        async Task AddProductAsync()
        {
            await _popupService.ShowAddProductPopupAsync(Products);
        }

        [RelayCommand]
        async Task GoToProductDetailsAsync()
        {

        }

        public async Task LoadProducts()
        {
            try
            {
                Products.Clear();
                var res = await _productsServices.GetProducts();

                foreach (var product in res)
                {
                    {
                        Products.Add(new Product
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Quantity = product.Quantity
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: { ex.Message}");
            }
        }
    }
}
