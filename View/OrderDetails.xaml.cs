using OrderApp.ViewModel;
using System.Threading.Tasks;

namespace OrderApp.View;

public partial class OrderDetails : ContentPage
{
    private OrderDetailsViewModel _orderDetailsViewModel;
    public OrderDetails(OrderDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _orderDetailsViewModel = viewModel;
    }
}