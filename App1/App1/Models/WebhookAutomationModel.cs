using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace App1.Models
{
    internal class WebhookAutomationModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) { return false; }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private string _automation_name;
        public string AutoName
        {
            get => _automation_name;

            set
            {
                SetProperty(ref _automation_name, value);
            }
        }
        private string _automation_webhook;
        public string AutoWebhook
        {
            get => _automation_webhook;

            set
            {
                SetProperty(ref _automation_webhook, value);
            }
        }

        public WebhookAutomationModel(string name, string webhook)
        {
            AutoName = name;
            AutoWebhook = webhook;
        }
    }
}
