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
            // if the product is already in the list of added product, get the product to the first location in the list of products In order
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
                await _orderServices.AddProductToOrder(Order, product, 1);
                await LoadProductsOfOrder();
            }
            else
            {
                // throw exception
                await Shell.Current.DisplayAlert("Failed", "Not enough stock quantity", "Ok");
            }
        }

        [RelayCommand]
        async Task AddProductToOrderAsync()
        {
            await _popupService.ShowAddProductToOrderPopupAsync(Order);
        }

        [RelayCommand]
        public async Task LoadItems()
        {
            IsBusy = true;
            // Give the UI a moment to update the ActivityIndicator
            await Task.Yield();

            await LoadCustomer();
            await LoadProductsOfOrder();
            await LoadAllProductsAsync();
            IsBusy = false;
        }

        [RelayCommand]
        async Task DeleteItem(ProductsInOrders productInOrder)
        {
            int quantityToRemove = productInOrder.Quantity;
            await _productService.UpdateProductStock(-1 * quantityToRemove, productInOrder.Product.Id);
            await _productInOrdersServices.DeleteProductInOrder(productInOrder.OrderId, productInOrder.Product.Id);
            ProductsInOrders.Remove(productInOrder);
        }

        public async Task LoadProductsOfOrder()
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

        public async Task LoadAllProductsAsync()
        {
            var products = await _productService.GetProducts();
            Products.Clear();
            foreach (var product in products)
                Products.Add(product);

            // Initially show all products
            FilteredProducts = new ObservableCollection<Product>(Products);
        }

        [RelayCommand]  
         async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var lower = searchText.ToLower();
                var results = Products.Where(p => p.Name.ToLower().Contains(lower)).ToList();
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
                Console.WriteLine("something went wrong");
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
            item.Product.Quantity--;
            RecalculateTotal();
        }

        [RelayCommand]
        void DecrementQuantity(ProductsInOrders item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity--;
                item.Product.Quantity++;
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
