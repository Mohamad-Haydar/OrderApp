using OrderApp.ViewModel;
using Syncfusion.Maui.Scheduler;

namespace OrderApp.View;

public partial class Events : ContentPage
{
    EventsViewModel _eventsViewModel;
    public Events(EventsViewModel viewModel)
	{
        InitializeComponent();
		BindingContext = viewModel;
        _eventsViewModel = viewModel;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _eventsViewModel.SelectDateAync(DateOnly.FromDateTime(DateTime.Now));
    }


    private async void OnSchedulerTapped(object sender, SchedulerTappedEventArgs e)
    {
        var appointments = e.Appointments;
        var selectedDate = e.Date;
        var schedulerElement = e.Element;
        var weekNumber = e.WeekNumber;
        await _eventsViewModel.SelectDateAync(DateOnly.FromDateTime(selectedDate.Value));
    }
}