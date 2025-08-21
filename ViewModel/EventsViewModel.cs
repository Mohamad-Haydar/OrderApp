using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using Plugin.LocalNotification;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class EventsViewModel : BaseViewModel
    {
        private EventsServices _eventsServices;
        private PopupService _popupService;
        public ObservableCollection<EventModel> Events { get; set; } = [];
        public ObservableCollection<EventModel> EventsOfDay { get; set; } = [];

        [ObservableProperty]
        string title;

        [ObservableProperty]
        DateOnly dateSelected;

        [ObservableProperty]
        DateTime dateTimeSelected;

        public EventsViewModel() 
        {

            _eventsServices = ServiceHelper.Resolve<EventsServices>();
            _popupService = ServiceHelper.Resolve<PopupService>();
            DateSelected = DateOnly.FromDateTime(DateTime.Now);
            Title = $"Events of {DateSelected}";
        }
        [RelayCommand]
        public async Task InitAsync()
        {
            try
            {
                await Task.Yield();
                IsBusy = true;
                await LoadAllEventsAsync();
                await SelectDateAsync(DateOnly.FromDateTime(DateTime.Now));
            }
            catch (Exception)
            {
                IsBusy = false;
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading Events. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadAllEventsAsync()
        {
            try
            {
                var eventsResult = await _eventsServices.GetAllEvents();
                Events.Clear();
                foreach (var item in eventsResult)
                {
                    item.Background = GetColorBrushForEventType(item.EventType);
                    item.BackgroundStr = GetBackgroundStrForEventType(item.EventType);
                    item.TextColor = GetColorForEventType(item.EventType);
                    Events.Add(item);
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading events. Please try again.", "OK");
            }
        }

        public async Task SelectDateAsync(DateOnly date)
        {
            try
            {
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
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading events. Please try again.", "OK");
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

        partial void OnDateTimeSelectedChanged(DateTime value)
        {
            // Whenever the user selects a date in the scheduler
            Task.Run(async () => await SelectDateAsync(DateOnly.FromDateTime(value))) ;
        }

        [RelayCommand]
        async Task AddEventAsync()
        {
            try
            {
                await _popupService.ShowAddEventPopupAsync();
                await SelectDateAsync(dateSelected);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding an event. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task EditEventAsync(EventModel eventModel)
        {
            try
            {
                if (eventModel == null) return;
                await _popupService.ShowEditEventPopupAsync(eventModel);
                await SelectDateAsync(dateSelected);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while editing the event. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task DeleteEventAsync(EventModel eventModel)
        {
            try
            {
                if (eventModel == null) return;

#if ANDROID
                // Cancel native AlarmManager alarm (UTC)
                Platforms.Android.AndroidAlarmScheduler.Cancel(
                        notificationId: eventModel.Id + 1000);
#else
               LocalNotificationCenter.Current.Cancel(eventModel.Id + 1000);
#endif


                var result = await _eventsServices.DeleteEvent(eventModel.Id);
                if (result)
                {
                    EventsOfDay.Remove(eventModel);
                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while deleting the event. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task SchedulerTappedAsync(SchedulerTappedEventArgs args)
        {
            if (args.Date != null)
            {
                await SelectDateAsync(DateOnly.FromDateTime(args.Date.Value));
            }
        }


    }
}
