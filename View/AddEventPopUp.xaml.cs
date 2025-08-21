using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class AddEventPopUp : Popup
{
    public AddEventPopUp()
    {
        InitializeComponent();
        this.BindingContext = new AddEventPopUpViewModel();
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