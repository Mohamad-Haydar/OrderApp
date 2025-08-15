using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        float total;
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
            Total = 0;
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
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding the product. Please try again.", "OK");
            }
        }

        [RelayCommand]
        public async Task LoadItems()
        {
            try
            {
                IsBusy = true;
            // Give the UI a moment to update the ActivityIndicator
                await Task.Yield();
                await LoadCustomer();
                await LoadAllProductsAsync();
                await LoadProductsOfOrder();
                IsBusy = false;
            }
            catch (Exception)
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading order details. Please try again.", "OK");
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
                Total = 0;
                ProductsInOrders.Clear();
                Total = await _productService.GetProductsInOrders(ProductsInOrders, Order);
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
                foreach (var product in products)
                    Products.Add(product);

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

        public async Task LoadCustomer()
        {
            try
            {
                ClientName = await _clientServices.GetCustomer(Order);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading the client information. Please try again.", "OK");
            }
        }

        [RelayCommand]
        void IncrementQuantity(ProductsInOrders item)
        {
            if(item.Product.Quantity < 1)
            {
                Shell.Current.DisplayAlert("Error", "Not enough stock available", "OK");
                return;
            }
            item.Quantity++;
            //item.Product.Quantity--;
            RecalculateTotal();
        }

        [RelayCommand]
        void DecrementQuantity(ProductsInOrders item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity--;
                //item.Product.Quantity++;
                RecalculateTotal();
            }
        }

        [RelayCommand]
        async Task UpdateOrderAsync()
        {
            try
            {
                await _orderServices.UpdateOrderAsync(ProductsInOrders);
                // to see it in the UI
                RecalculateTotal();
                // TO update it in the database
                Order.Total = Total;
                await _orderServices.SetTotalAsync(Order);
                await Shell.Current.DisplayAlert("Success", "The product is updated successfully", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating the order. Please try again.", "OK");
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
