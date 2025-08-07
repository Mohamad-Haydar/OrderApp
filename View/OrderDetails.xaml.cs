using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class OrderDetails : ContentPage
{
    private OrderDetailsViewModel _orderDetailsViewModel;
    public OrderDetails(OrderDetailsViewModel viewModel, OrderDetailsViewModel orderDetailsViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _orderDetailsViewModel = orderDetailsViewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _orderDetailsViewModel.LoadCustomer();
        await _orderDetailsViewModel.LoadProducts();
    }
}