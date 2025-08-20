using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    [QueryProperty(nameof(Order), nameof(Order))]
    public partial class OrderDetailsViewModel : BaseViewModel
    {
        public IEnumerable<Product> Products { get; set; } = [];

        [ObservableProperty]
        ObservableCollection<Product> filteredProducts =[];

        [ObservableProperty]
        string searchText;

        [ObservableProperty]
        Order order;

        [ObservableProperty]
        string title;
        [ObservableProperty]
        string? clientName;

        public ProductsServices _productService;
        private readonly ProductInOrdersServices _productInOrdersServices;
        public OrderServices _orderServices;
        private bool _isDataLoaded = false;

        public OrderDetailsViewModel() 
        {
            _productService = ServiceHelper.Resolve<ProductsServices>();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
            _productInOrdersServices = ServiceHelper.Resolve<ProductInOrdersServices>();
        }

        [RelayCommand]
        async Task AddProductAsync(Product product)
        {
            try
            {
                var existingProduct = Order.Products.FirstOrDefault(x => x.Product.Id == product.Id);
                if (existingProduct != null)
                {
                // Remove it from its current position
                    Order.Products.Remove(existingProduct);

                // Insert it at the first position
                    Order.Products.Insert(0,existingProduct);
                    return;
                }
                var availableStock = await _productService.GetStuckQuantity(product.Id);

                // If not enough stock, stop here
                if (availableStock > 0)
                {
                // add only 1 item of the product 
                    Order.Products.Insert(0, new ProductsInOrders()
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
                Order.CalculateTotal();
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding the product. Please try again.", "OK");
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
                Order.Products.Remove(productInOrder);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while deleting the product from the order. Please try again.", "OK");
            }
        }

        public async Task LoadDataAsync()
        {

            if (Order == null || _isDataLoaded) return;

            try
            {
                IsBusy = true;
                _isDataLoaded = true;
                await Task.Delay(700);

                var productsres = await _productService.GetProducts();
                var productsIOres = await _productService.GetProductsInOrders(Order);

                // Always clear and replace to avoid showing old values
                Products = productsres;
                FilteredProducts = new ObservableCollection<Product>(productsres);
                if(!Order.IsLoaded)
                    Order.Products = new ObservableCollection<ProductsInOrders>(productsIOres);

                Title = $"Order #{Order.Id}" +
                        (string.IsNullOrEmpty(Order.ClientName) ? "" : $" - {Order.ClientName}");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading products of the order. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                Order.IsLoaded = true;
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
           // Order.UpdateTotal(item.Product.Price);
            //Order.CalculateTotal();
        }

        [RelayCommand]
        void DecrementQuantity(ProductsInOrders item)
        {
            if (item.Quantity > 0)
            {
                item.Quantity--;
                //Order.UpdateTotal(-1 * item.Product.Price);
                //Order.CalculateTotal();
            }
        }

        [RelayCommand]
        async Task UpdateOrderAsync()
        {
            var connection = AdoDatabaseService.GetConnection();
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();
            try
            {

                await _orderServices.UpdateOrderAsync(Order.Products, connection,  transaction);
                // TO update it in the database
                await _orderServices.SetTotalAsync(Order, connection, transaction);
                await Shell.Current.DisplayAlert("Success", "The product is updated successfully", "Ok");
            }
            catch (Exception)
            {
                await transaction?.RollbackAsync();
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating the order. Please try again.", "OK");
            }
            finally
            {
                await transaction?.CommitAsync();
                await connection.CloseAsync();
            }
        }
    }
}
