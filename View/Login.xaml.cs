using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class Login : ContentPage
{
	public Login(LoginViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}