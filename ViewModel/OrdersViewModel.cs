using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.View;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class OrdersViewModel : BaseViewModel
    {
        private readonly PopupService _popupService;
        private readonly OrderServices _orderServices;
        public ObservableCollection<Order> Orders { get; }

        public OrdersViewModel(PopupService popupService, LocalizationService localizationService, ThemeService themeService, OrderServices orderServices) : base(localizationService, themeService)
        {
            Orders = [];
            _popupService = popupService;
            _orderServices = orderServices;
        }



        [RelayCommand]
        async Task AddOrderAsync()
        {
            // call the popup service to create new popup to add an order
            await _popupService.ShowAddOrderPopupAsync(Orders);
        }

        // To delete an order from the database
        [RelayCommand]
        async Task DeleteOrderAsync(Order order)
        {
            try
            {
                await _orderServices.DeleteOrder(order);
                await LoadOrders();
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
        
        // Get the orders from the database and update the view
        public async Task LoadOrders()
        {
            try
            {
                Orders.Clear(); // clear the orders to remove all of them from the view
                var res = await _orderServices.GetOrders();
                foreach (var item in res)
                {
                    Orders.Add(new Order
                    {
                        Id = item.Id,
                        ClientId = item.ClientId,
                        Total = item.Total,
                        DateToPick = item.DateToPick,
                        ClientName = item.ClientName
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
