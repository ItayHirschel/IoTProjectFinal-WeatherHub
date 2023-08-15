using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using App1.Interfaces;

namespace App1.Services
{
    class NotificationService
    {
        public readonly ILocalNotificationService localNotificationService;

        public NotificationService()
        {
            localNotificationService = DependencyService.Get<ILocalNotificationService>();
        }

        public void ShowNotification(string title, string message)
        {
            localNotificationService.ShowNotification(title, message, new Dictionary<string,string>());
        }
        public void ShowNotification(string title, string message, string dev_name)
        {
            Dictionary<string,string> data = new Dictionary<string,string>();
            data["Device"] = dev_name;
            localNotificationService.ShowNotification(title, message, data);
        }
    }
}
