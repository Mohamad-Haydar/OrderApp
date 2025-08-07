using CommunityToolkit.Maui.Views;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddEventPopUp : Popup
{
	public AddEventPopUp(ObservableCollection<EventModel> events, DateOnly dateSelected)
	{
		InitializeComponent();
		this.BindingContext = new AddEventPopUpViewModel( this, events, new EventsServices(), dateSelected);
	}
}