using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class Orders : ContentPage
{
    private OrdersViewModel _ordersViewModel;
    public Orders(OrdersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _ordersViewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _ordersViewModel.LoadOrders();
    }
}