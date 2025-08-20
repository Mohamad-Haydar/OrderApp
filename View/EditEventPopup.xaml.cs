using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;


public partial class EditEventPopup : Popup
{
    public EditEventPopup(EventModel oldEvent)
    {
        InitializeComponent();
        this.BindingContext = new EditEventPopUpViewModel(oldEvent);
        // Prevent the popup from closing when tapping outside
        CanBeDismissedByTappingOutsideOfPopup = false;
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }
}