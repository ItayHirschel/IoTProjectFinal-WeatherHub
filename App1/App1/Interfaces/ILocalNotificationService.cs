using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Interfaces
{
    public interface ILocalNotificationService
    {
        void ShowNotification(string title, string message, IDictionary<string,string> data);
    }
}
