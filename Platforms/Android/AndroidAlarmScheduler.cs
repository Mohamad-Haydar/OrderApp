using Android.App;
using Android.Content;


namespace OrderApp.Platforms.Android
{
    public static class AndroidAlarmScheduler
    {
        public static void Schedule(
            int notificationId,
            string title,
            string message,
            DateTime triggerAtLocal)
        {
            // Build Intent to AlarmReceiver
            var intent = new Intent(Platform.AppContext, typeof(AlarmReceiver));
            intent.PutExtra("notificationId", notificationId);
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);

            var pending = PendingIntent.GetBroadcast(
                Platform.AppContext,
                notificationId,
                intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

            var alarmManager = (AlarmManager)(Platform.AppContext.GetSystemService(Context.AlarmService));

            // Convert to milliseconds since Unix epoch UTC
            long triggerMillis = new DateTimeOffset(triggerAtLocal).ToUnixTimeMilliseconds();

            var info = new AlarmManager.AlarmClockInfo(triggerMillis, pending);
            // Use SetExactAndAllowWhileIdle for Doze mode support
          
            alarmManager.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                triggerMillis,
                pending);
        }
    }
}
