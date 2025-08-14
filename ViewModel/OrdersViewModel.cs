using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.View;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OrderApp.ViewModel
{
    public partial class OrdersViewModel : BaseViewModel
    {
        private readonly PopupService _popupService;
        private readonly OrderServices _orderServices;
        [ObservableProperty]
        ObservableCollection<Order> orders;
        public ObservableCollection<Client> SelectedClient { get; set; } = new();
        // static information for the multy selection
        public ObservableCollection<Client> Result { get; set; } = new();

        public OrdersViewModel()
        {
            Orders = [];
            _popupService = ServiceHelper.Resolve<PopupService>();
            _orderServices = ServiceHelper.Resolve<OrderServices>();
            LoadAsync();
        }



        [RelayCommand]
        async Task AddOrderAsync()
        {
            // call the popup service to create new popup to add an order
            await _popupService.ShowAddOrderPopupAsync();
        }

        // To delete an order from the database
        [RelayCommand]
        async Task DeleteOrderAsync(Order order)
        {
            try
            {
                // Remove the order from the database
                await _orderServices.DeleteOrder(order.Id);
                // After deleting the order, SHOW IN THE VIEW the updated orders
                //await LoadOrders();
                Orders.Remove(order); 
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong");
            }

        }

        [RelayCommand]
        async Task GoToOrderDetailsAsync(Order order)
        {
            await Shell.Current.GoToAsync(nameof(OrderDetails), true, new Dictionary<string, object>
            {
                {nameof(Order), order},
            });
        }

        [RelayCommand]
        async Task CountriesChanged()
        {
            // Do something with SelectedCountries
            Debug.WriteLine("Countries changed:");
            foreach (var c in SelectedClient)
                Debug.WriteLine($" - {c}");
        }

        public static Func<Client, string> ClientDisplayProperty => c => c.Name;


        // Get the orders from the database and update the view
        public async Task LoadOrders()
        {
            try
            {
                Orders.Clear(); // clear the orders to remove all of them from the view
                var res = await _orderServices.GetOrders();
                Orders = new ObservableCollection<Order>(res);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void LoadAsync()
        {
            Result.Clear();
            Result.Add(new Client() { Id=1,Name="ali", Details= "client details client 1" });
            Result.Add(new Client() { Id=2,Name="mohamad", Details= "client details client 2" });
            Result.Add(new Client() { Id=3,Name="jawad", Details= "client details client 3" });
        }
    }
}
