using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class OrderDetails : ContentPage
{
    private OrderDetailsViewModel _orderDetailsViewModel;
    public OrderDetails(OrderDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _orderDetailsViewModel = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _orderDetailsViewModel.LoadItems();
        //await _orderDetailsViewModel.LoadCustomer();
        //await _orderDetailsViewModel.LoadProductsOfOrder();
        //await _orderDetailsViewModel.LoadAllProductsAsync();
    }
}