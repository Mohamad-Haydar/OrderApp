using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class Orders : ContentPage
{
    private OrdersViewModel _ordersViewModel;
    public Orders(OrdersViewModel viewModel, OrdersViewModel ordersViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _ordersViewModel = ordersViewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _ordersViewModel.LoadOrders();
    }
}