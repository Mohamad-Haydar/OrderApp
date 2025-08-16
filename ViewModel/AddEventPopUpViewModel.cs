using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Exceptions;
using OrderApp.Helper;
using OrderApp.Services;
using Plugin.LocalNotification;

namespace OrderApp.ViewModel
{
    public partial class AddEventPopUpViewModel : ObservableObject
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

        private EventsServices _eventsServices;

        public AddEventPopUpViewModel()
        {
            _eventsServices = ServiceHelper.Resolve<EventsServices>();
            EventTypes = ["Meeting", "Vacation", "Call"];
            startTime = DateTime.Now.TimeOfDay;
            EndTime = DateTime.Now.TimeOfDay;
            startDate = DateTime.Now.Date;
            endDate = startDate;
        }

      
        [RelayCommand]
        async Task AddEventAsync()
        {
            try
            {
                var startDateTime = StartDate.Date + StartTime;
                var endDateTime = EndDate.Date + EndTime;
                // set the event in the database
                int addedId = await _eventsServices.AddEvent(EventName, Description, SelectedEventType, startDateTime.ToString("yyyy-MM-dd HH:mm:ss"), endDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (addedId == 0)
                    return;
/*#if ANDROID || IOS
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var fcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

                if (!string.IsNullOrEmpty(fcmToken))
                {
                    // 3. Prepare the payload to send to your Web API
                    var notificationPayload = new
                    {
                        Token = fcmToken,
                        Title = "New Event Added!",
                        Body = $"Event: {EventName} on {startDateTime}"
                    };

                    // 4. Send the notification request to .NET Core Web API
                    var response = await _httpClient.PostAsJsonAsync("api/notifications/send", notificationPayload);
                 
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("FCM notification request sent successfully to backend.");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to send FCM notification request: {response.StatusCode} - {errorContent}");
                    }

                    //Console.WriteLine($"FCM token: {token}");

                    //// share token via system dialog
                    //await Share.RequestAsync(new ShareTextRequest
                    //{
                    //    Text = token,
                    //    Title = "Share Firebase Token"
                    //});
#endif*/
                 
                var reminderTime = startDateTime.AddSeconds(-15);

                if (reminderTime > DateTime.Now)
                {
                    // set the notification for the event
#if ANDROID
                    // Schedule native AlarmManager alarm (UTC)
                    Platforms.Android.AndroidAlarmScheduler.Schedule(
                        notificationId: addedId + 1000,
                        title: $"Upcoming: {EventName}",
                        message: $"Starts at {startDateTime}",
                        triggerAtLocal: reminderTime);

#else
                    var request = new NotificationRequest
                    {
                        NotificationId =  addedId + 1000,
                        Title = "Upcoming: " + EventName,
                        Description = $"Starts at {startDateTime:t}",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = reminderTime,
                        },
                    };

                    await LocalNotificationCenter.Current.Show(request);
#endif
                }

                await Shell.Current.DisplayAlert("Success", "Event added successfully!", "OK");
                // Signal to close the popup
                WeakReferenceMessenger.Default.Send(new ClosePopupMessage());
            }
            catch (ValidationException vex)
            {
                await Shell.Current.DisplayAlert("Validation Error", vex.Message, "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "Unable to add Event please try again", "OK");
            }
            
        }
    }
}
