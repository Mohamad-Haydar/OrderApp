using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Collections;
using OrderApp.View;

namespace OrderApp.ViewModel
{
    [QueryProperty(nameof(Order), nameof(Order))]
    public partial class OrderDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<Product> Products { get; set; } = new();

        [ObservableProperty]
        ObservableCollection<Product> filteredProducts = new();

        [ObservableProperty]
        string searchText;

        [ObservableProperty]
        ObservableCollection<ProductsInOrders> productsInOrders;

        [ObservableProperty]
        Order order;
        [ObservableProperty]
        string title;
        [ObservableProperty]
        string? clientName;

        public PopupService _popupService;
        public ProductsServices _productService;
        private readonly ProductInOrdersServices _productInOrdersServices;
        public ClientServices _clientServices;
        public OrderServices _orderServices;

        public OrderDetailsViewModel() 
        {
            _popupService = ServiceHelper.Resolve<PopupService>();
            ProductsInOrders = [];
            _productService = ServiceHelper.Resolve<ProductsServices>();
            _clientServices = ServiceHelper.Resolve<ClientServices>();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
            _productInOrdersServices = ServiceHelper.Resolve<ProductInOrdersServices>(); 
        }

        [RelayCommand]
        async Task AddProductAsync(Product product)
        {
            try
            {
                var existingProduct = ProductsInOrders.FirstOrDefault(x => x.Product.Id == product.Id);
                if (existingProduct != null)
                {
                // Remove it from its current position
                    ProductsInOrders.Remove(existingProduct);

                // Insert it at the first position
                    ProductsInOrders.Insert(0,existingProduct);
                    return;
                }
                var availableStock = await _productService.GetStuckQuantity(product.Id);

                // If not enough stock, stop here
                if (availableStock > 0)
                {
                // add only 1 item of the product 
                    ProductsInOrders.Insert(0, new ProductsInOrders()
                    {
                        Id = -1,
                        Product = product,
                        OrderId = Order.Id,
                        Quantity = 1
                    });
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failed", "Not enough stock quantity", "Ok");
                }
                Order.CalculateTotal(ProductsInOrders);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding the product. Please try again.", "OK");
            }
        }

        partial void OnOrderChanged(Order value)
        {
            _ = LoadItems(); // fire-and-forget, does not block UI thread
        }

        private async Task LoadItems()
        {
            try
            {
                IsBusy = true;
                // Give the UI a moment to update the ActivityIndicator
                await Task.Yield();
                var loadProductsTask = LoadAllProductsAsync();
                var loadProductsOfOrderTask = LoadProductsOfOrder();
                await Task.WhenAll(loadProductsTask, loadProductsOfOrderTask);
            }
            catch (Exception)
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading order details. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task DeleteItem(ProductsInOrders productInOrder)
        {
            try
            {
                int quantityToRemove = productInOrder.Quantity;
                await _productService.UpdateProductStock(-1 * quantityToRemove, productInOrder.Product.Id);
                await _productInOrdersServices.DeleteProductInOrder(productInOrder.OrderId, productInOrder.Product.Id);
                ProductsInOrders.Remove(productInOrder);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while deleting the product from the order. Please try again.", "OK");
            }
        }

        public async Task LoadProductsOfOrder()
        {
            try
            {
                Title = "Details of Order: " + Order.Id;
                ProductsInOrders.Clear();
                await _productService.GetProductsInOrders(ProductsInOrders, Order);
                Order.CalculateTotal(ProductsInOrders);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading products of the order. Please try again.", "OK");
            }
        }

        public async Task LoadAllProductsAsync()
        {
            try
            {
                var products = await _productService.GetProducts();
                Products.Clear();
                Products = new ObservableCollection<Product>(products);

                // Initially show all products
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading all products. Please try again.", "OK");
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var results = Products.Where(p => p.Name.ToLower().Contains(value));
                FilteredProducts = new ObservableCollection<Product>(results);
            }
        }

        [RelayCommand]
        void IncrementQuantity(ProductsInOrders item)
        {
            if(!item.Product.HasStock())
            {
                Shell.Current.DisplayAlert("Error", "Not enough stock available", "OK");
                return;
            }
            item.Quantity++;
            Order.CalculateTotal(ProductsInOrders);
        }

        [RelayCommand]
        void DecrementQuantity(ProductsInOrders item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity--;
                //item.Product.Quantity++;
                //RecalculateTotal();
                Order.CalculateTotal(ProductsInOrders);
            }
        }

        [RelayCommand]
        async Task UpdateOrderAsync()
        {
            try
            {
                await _orderServices.UpdateOrderAsync(ProductsInOrders);
                // to see it in the UI
                Order.CalculateTotal(ProductsInOrders);
                // TO update it in the database
                await _orderServices.SetTotalAsync(Order);
                await Shell.Current.DisplayAlert("Success", "The product is updated successfully", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating the order. Please try again.", "OK");
            }
        }
    }
}
