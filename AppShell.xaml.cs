using OrderApp.View;
using OrderApp.ViewModel;

namespace OrderApp
{
    public partial class AppShell : Shell
    {
        public AppShell(ShellViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            Routing.RegisterRoute(nameof(Orders), typeof(Orders));
            Routing.RegisterRoute(nameof(OrderDetails), typeof(OrderDetails));
            Routing.RegisterRoute(nameof(Clients), typeof(Clients));
            Routing.RegisterRoute(nameof(Products), typeof(Products));
        }
    }
}
