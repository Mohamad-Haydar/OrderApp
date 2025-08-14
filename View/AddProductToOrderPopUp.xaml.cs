using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddProductToOrderPopUp : Popup
{
	public AddProductToOrderPopUp(ObservableCollection<Product> products, Order order)
	{
		InitializeComponent();
        this.BindingContext = new AddProductToOrderPopupViewModel(products, order);
        // Prevent the popup from closing when tapping outside
        CanBeDismissedByTappingOutsideOfPopup = false;
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        if (this.BindingContext is AddProductToOrderPopupViewModel viewModel)
        {
            await viewModel.AddProductToOrderCommand.ExecuteAsync(null);

            await this.CloseAsync();
        }
    }

}