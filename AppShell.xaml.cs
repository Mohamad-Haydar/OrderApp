using OrderApp.View;
using OrderApp.ViewModel;

namespace OrderApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Orders), typeof(Orders));
            Routing.RegisterRoute(nameof(OrderDetails), typeof(OrderDetails));
            Routing.RegisterRoute(nameof(Clients), typeof(Clients));
            Routing.RegisterRoute(nameof(Products), typeof(Products));
        }

        protected override void OnAppearing()
        {
            /*
             * the binding context is set in the on Appearing method 
             * because when the ShellViewModel is created, the BaseViewModel is created before
             * which calls the localization service that tries to get the  Shell.Current.Resources and in 
             * this case the Shell.Current is null, so it don't set the language and use the default language.
             */
            base.OnAppearing();

            if (BindingContext == null)
            {
                // Now it's safe to create the ViewModel
                BindingContext = new ShellViewModel();
            }
        }
    }
}
