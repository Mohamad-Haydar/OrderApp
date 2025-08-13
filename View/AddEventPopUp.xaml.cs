using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddEventPopUp : Popup
{
	public AddEventPopUp()
	{
		InitializeComponent();
		this.BindingContext = new AddEventPopUpViewModel();
	}

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (this.BindingContext is AddEventPopUpViewModel viewModel)
        {
            await viewModel.AddEventCommand.ExecuteAsync(null);

            await this.CloseAsync();
        }
    }
}