using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidApp = Android.App.Application;
using App1.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(App1.Droid.Services.LocalNotificationService))]
namespace App1.Droid.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        private const string CHANNEL_ID = "local_notification_channel";
        private const string CHANNEL_NAME = "notifications";
        private const string CHANNEL_DESCRIPTION = "local notification channel description";

        private int notificationId = -1;
        private const string TITLE_KEY = "title";
        private const string MESSAGE_KEY = "message";

        private bool isChannelInitialized = false;

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Default)
            {
                Description = CHANNEL_DESCRIPTION
            };

            var notificationManager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

            notificationManager.CreateNotificationChannel(channel);
        }

        public void ShowNotification(string title, string message, IDictionary<string, string> data)
        {
            if (!isChannelInitialized)
            {
                CreateNotificationChannel();
            }

            var intent = new Intent(AndroidApp.Context, typeof(MainActivity));
            intent.PutExtra(TITLE_KEY, title);
            intent.PutExtra(MESSAGE_KEY, message);

            intent.AddFlags(ActivityFlags.SingleTop);

            foreach(var key in data.Keys ) 
            {
                intent.PutExtra(key, data[key]);
            }

            notificationId++;

            var pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, notificationId, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new NotificationCompat.Builder(AndroidApp.Context, CHANNEL_ID)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Mipmap.icon);

            var notificationManager = NotificationManagerCompat.From(AndroidApp.Context);
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }
    }
}