using Android.Content;
using AndroidX.Core.App;

namespace OrderApp.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true, DirectBootAware = true)]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

                 // 1. Get the data from the intent using the correct keys
                int notificationId = intent.GetIntExtra("notificationId", 0);
                string title = intent.GetStringExtra("title") ?? "Reminder";
                string message = intent.GetStringExtra("message") ?? "Event Reminder";

                var notificationManager = NotificationManagerCompat.From(context);

                // 2. Build the notification (channel already exists from MainActivity)
                var builder = new NotificationCompat.Builder(context, "event_channel")
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetSmallIcon(Resource.Drawable.notification_bg)
                    .SetPriority(NotificationCompat.PriorityHigh)
                    .SetAutoCancel(true);

                // 3. Use the notificationId from the intent
                notificationManager.Notify(notificationId, builder.Build());
         
        }
    }
}
