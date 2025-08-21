using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Model;
using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class EditProductPopup : Popup
{
	public EditProductPopup(Product product)
	{
        InitializeComponent();
        this.BindingContext = new EditProductPopUpViewModel(product);
        // Prevent the popup from closing when tapping outside
        // Subscribe to close message
        WeakReferenceMessenger.Default.Register<ClosePopupMessage>(this, async (r, m) =>
        {
            await this.CloseAsync();
        });
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }
}