using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using Syncfusion.Maui.Core.Carousel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddOrderPopup : Popup
{
    public AddOrderPopup(ObservableCollection<Client> clients)
	{
		InitializeComponent();
        this.BindingContext = new AddOrderPopupViewModel(clients);
        // Prevent the popup from closing when tapping outside
        CanBeDismissedByTappingOutsideOfPopup = false;
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (this.BindingContext is AddOrderPopupViewModel viewModel)
        {
           await viewModel.AddOrderCommand.ExecuteAsync(null);

            await this.CloseAsync();
        }
    }
}