using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
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