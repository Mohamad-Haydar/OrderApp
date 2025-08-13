using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddClientPopup : Popup
{
    public AddClientPopup(ObservableCollection<Client> clients)
    {
        InitializeComponent();
        this.BindingContext = new AddClientPopupViewModel(clients);
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (this.BindingContext is AddClientPopupViewModel viewModel)
        {
            await viewModel.AddClientCommand.ExecuteAsync(null);

            await this.CloseAsync();
        }
    }
}