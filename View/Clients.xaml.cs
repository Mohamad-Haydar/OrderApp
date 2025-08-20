using OrderApp.Services;
using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class Clients : ContentPage
{
    private ClientsViewModel _clientsViewModel;
    public Clients(ClientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _clientsViewModel = viewModel;
    }
}