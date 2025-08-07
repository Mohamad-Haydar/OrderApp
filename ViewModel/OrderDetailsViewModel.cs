using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    [QueryProperty(nameof(Order), nameof(Order))]
    public partial class OrderDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Order order;

        [ObservableProperty]
        ObservableCollection<ProductsInOrders> productsInOrders;

        [ObservableProperty]
        float total;

        [ObservableProperty]
        string title;

        [ObservableProperty]
        string? clientName;

        public PopupService _popupService;
        public ProductsServices _productService;
        public ClientServices _clientServices;
        public OrderServices _orderServices;
        public OrderDetailsViewModel(PopupService popupService, LocalizationService localizationService, ThemeService themeService, ProductsServices productService, ClientServices clientServices, OrderServices orderServices) : base(localizationService, themeService)
        {
            _popupService = popupService;
            ProductsInOrders = [];
            Total = 0;
            _productService = productService;
            _clientServices = clientServices;
            _orderServices = orderServices;
        }


        [RelayCommand]
        async Task AddProductToOrderAsync()
        {
            await _popupService.ShowAddProductToOrderPopupAsync(Order);
        }

        public async Task LoadProducts()
        {
            Title = "Details of Order: " + Order.Id;
            Total = 0;
            ProductsInOrders.Clear();
            try
            {
                Total = await _productService.GetProductsInOrders(ProductsInOrders, Order);
            }
            catch (Exception)
            {
                Console.WriteLine("something went wrong");
            }
        }
    
        public async Task LoadCustomer()
        {
            try
            {
                ClientName = await _clientServices.GetCustomer(Order);
            }
            catch (Exception)
            {
                Console.WriteLine("something went wrong");
            }
        }

        [RelayCommand]
        void IncrementQuantity(ProductsInOrders item)
        {
            item.Quantity++;
            RecalculateTotal();
        }

        [RelayCommand]
        void DecrementQuantity(ProductsInOrders item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity--;
                RecalculateTotal();
            }
        }

        [RelayCommand]
        async Task UpdateOrderAsync()
        {
            try
            {
                await _orderServices.UpdateOrderAsync(ProductsInOrders);
                RecalculateTotal();
                Order.Total = Total;
                await _orderServices.SetTotalAsync(Order);
                await Shell.Current.DisplayAlert("Success", "The product is updated successfully", "Ok");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Database error: {ex.Message}", "OK");
            }
        }

        void RecalculateTotal()
        {
            Total = 0;
            foreach (var item in ProductsInOrders)
            {
                Total += item.Quantity * item.Product.Price;
            }
        }
    }
}
