using OrderApp.ViewModel;
using Syncfusion.Maui.Scheduler;

namespace OrderApp.View;

public partial class Events : ContentPage
{
    EventsViewModel _eventsViewModel;
    public Events(EventsViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = _eventsViewModel = viewModel;
	}
}