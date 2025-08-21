using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace OrderApp.ViewModel
{
    public partial class EditEventPopUpViewModel : ObservableObject
    {
        [ObservableProperty]
        public string eventName;
        [ObservableProperty]
        public string description;
        [ObservableProperty]
        public List<string> eventTypes;
        [ObservableProperty]
        public string selectedEventType;
        [ObservableProperty]
        public DateTime startDate;
        [ObservableProperty]
        public TimeSpan startTime;
        [ObservableProperty]
        public DateTime endDate;
        [ObservableProperty]
        public TimeSpan endTime;
        [ObservableProperty]
        DateTime today = DateTime.Today;

        private readonly int userId;

        private readonly Popup _popup;
        private EventsServices _eventsServices;
        private EventModel _oldEvent = new();
        public EditEventPopUpViewModel(EventModel oldEvent)
        {
            _popup = ServiceHelper.Resolve<Popup>();
            userId = Preferences.Get("UserId", 0);
            _eventsServices = ServiceHelper.Resolve<EventsServices>();
            EventName = oldEvent.EventName;
            Description = oldEvent.Description;
            EventTypes = ["Meeting", "Vacation", "Call"];
            startTime = oldEvent.From.TimeOfDay;
            EndTime = oldEvent.To.TimeOfDay;
            SelectedEventType = oldEvent.EventType;
            startDate = oldEvent.From.Date;
            _oldEvent = oldEvent;
        }
      
        [RelayCommand]
        async Task EditEventAsync()
        {
            try
            {

                var startDateTime = StartDate.Date + StartTime;
                var endDateTime = EndDate.Date + EndTime;
                var reminderTime = startDateTime.AddSeconds(-15);

                // Call the service to edit the event
                var updatedId = await _eventsServices.UpdateEvent(_oldEvent.Id, EventName, Description, SelectedEventType, startDateTime.ToString("yyyy-MM-dd HH:mm:ss"), endDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (updatedId == 0)
                    return;

                if (reminderTime > DateTime.Now)
                {
#if ANDROID
                    // Schedule native AlarmManager alarm (UTC)
                    Platforms.Android.AndroidAlarmScheduler.ReSchedule(
                            notificationId: _oldEvent.Id + 1000,
                            title: $"Upcoming: {EventName}",
                            message: $"Starts at {startDateTime}",
                            newTriggerAtLocal: reminderTime);

#else
                var request = new NotificationRequest
                {
                    NotificationId = _oldEvent.Id + 1000,
                    Title = "Upcoming: " + EventName,
                    Description = $"Starts at {startDateTime:t}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = reminderTime,
                    },
                };

                await LocalNotificationCenter.Current.Show(request);
#endif

                    WeakReferenceMessenger.Default.Send(new ClosePopupMessage());

                }

                await Shell.Current.DisplayAlert("Success", "Event updated successfully!", "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating the event. Please try again.", "OK");
            }

        }

    }
}
