using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Model;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddProductPopup : Popup
{
    public AddProductPopup(ObservableCollection<Product> products, Product product, int mode)
    {
        InitializeComponent();
        this.BindingContext = new AddProductPopupViewModel(products, product, mode);
        // Prevent the popup from closing when tapping outside
        CanBeDismissedByTappingOutsideOfPopup = false;

        // Subscribe to close message
        WeakReferenceMessenger.Default.Register<ClosePopupMessage>(this, async (r, m) =>
        {
            await this.CloseAsync();
            WeakReferenceMessenger.Default.Unregister<ClosePopupMessage>(this);
        });
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
        WeakReferenceMessenger.Default.Unregister<ClosePopupMessage>(this);
    }
}