using Android.App;
using Android.Content;
using OrderApp.Exceptions;
using System.Diagnostics;


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
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetClientsForPopup: {ex}");
                throw new ValidationException("Unable to Schedule an event, please try again");
            }
        }

        public static void ReSchedule(int notificationId, string title, string message, DateTime newTriggerAtLocal)
        {

            try
            {
                // Cancel the existing alarm
                Cancel(notificationId);

                // Schedule the new alarm
                Schedule(notificationId, title, message, newTriggerAtLocal);
            }
            catch (ValidationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetClientsForPopup: {ex}");
                throw new ValidationException("Unable to ReSchedule an event, please try again");
            }
        }

        public static void Cancel(int notificationId)
        {
            try
            {
                var intent = new Intent(Platform.AppContext, typeof(AlarmReceiver));
                var pending = PendingIntent.GetBroadcast(
                    Platform.AppContext,
                    notificationId,
                    intent,
                    PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

                var alarmManager = (AlarmManager)(Platform.AppContext.GetSystemService(Context.AlarmService));
                alarmManager.Cancel(pending);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error in GetClientsForPopup: {ex}");
                throw new ValidationException("Unable to Cancel an event, please try again");
            }
        }

    }
}
