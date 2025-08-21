using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Model;
using OrderApp.ViewModel;

namespace OrderApp.View;


public partial class EditEventPopup : Popup
{
    public EditEventPopup(EventModel oldEvent)
    {
        InitializeComponent();
        this.BindingContext = new EditEventPopUpViewModel(oldEvent);
        // Prevent the popup from closing when tapping outside
        CanBeDismissedByTappingOutsideOfPopup = false;

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