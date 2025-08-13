using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class EventsViewModel : BaseViewModel
    {
        private EventsServices _eventsServices;
        private PopupService _popupService;
        public ObservableCollection<EventModel> Events { get; set; }
        public ObservableCollection<EventModel> EventsOfDay { get; set; } = [];

        [ObservableProperty]
        string title;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DateUi))]
        DateOnly dateSelected;

        public DateOnly DateUi => DateSelected;


        public EventsViewModel() 
        {

            _eventsServices = ServiceHelper.Resolve<EventsServices>();
            _popupService = ServiceHelper.Resolve<PopupService>();
            DateSelected = DateOnly.FromDateTime(DateTime.Now);
            Title = $"Events of {DateSelected}";
        }

        public async Task SelectDateAync(DateOnly date)
        {
            //TappedCommand = "{Binding SelectDateAyncCommand}"
            DateSelected = date;
            Title = $"Events of {dateSelected}";
            var eventsResult = await _eventsServices.GetEvents(date);
            EventsOfDay.Clear();
            foreach (var item in eventsResult)
            {
                item.Background = GetColorBrushForEventType(item.EventType);
                item.BackgroundStr = GetBackgroundStrForEventType(item.EventType);
                item.TextColor = GetColorForEventType(item.EventType);
                EventsOfDay.Add(item);
            }
        }

        private Color GetColorForEventType(string eventType)
        {
            return eventType switch
            {
                "Meeting" => Colors.Blue,
                "Vacation" => Colors.Green,
                "Call" => Colors.Orange,
                _ => Colors.Gray,
            };
        }

        private Brush GetColorBrushForEventType(string eventType)
        {
            return eventType switch
            {
                "Meeting" => Brush.Blue,
                "Vacation" => Brush.Green,
                "Call" => Brush.Orange,
                //_ => Brush.Gray,
            };
        }

        private string GetBackgroundStrForEventType(string eventType)
        {
            return eventType switch
            {
                "Meeting" => "Blue",
                "Vacation" => "Green",
                "Call" => "Orange",
                //_ => Brush.Gray,
            };
        }

        [RelayCommand]
        async Task AddEventAsync()
        {
            // call the popup service to create new popup to add an Event
            await _popupService.ShowAddEventPopupAsync();
            await SelectDateAync(dateSelected);
        }

        [RelayCommand]
        async Task EditEventAsync(EventModel eventModel)
        {
            if (eventModel == null) return;
            await _popupService.ShowEditEventPopupAsync(eventModel);
            // cancel the notification
            //LocalNotificationCenter.Current.Cancel(eventModel.Id + 1000);

            //// Call the service to edit the event
            //var result = await _eventsServices.UpdateEvent(eventModel.Id, eventModel.EventName, eventModel.Description, eventModel.EventType, eventModel.From.ToString("yyyy-MM-dd HH:mm:ss"), eventModel.To.ToString("yyyy-MM-dd HH:mm:ss"));
            await SelectDateAync(dateSelected);
        }


        [RelayCommand]
        async Task DeleteEventAsync(EventModel eventModel)
        {
            if (eventModel == null) return;
            // cancel the notification
            LocalNotificationCenter.Current.Cancel(eventModel.Id + 1000);

            // Call the service to delete the event
            var result = await _eventsServices.DeleteEvent(eventModel.Id);
            if (result)
            {
                EventsOfDay.Remove(eventModel);
            }
        }
    }
}
