using App1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using App1.Services;

namespace App1.ViewModels
{
    internal class WebhookListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<WebhookAutomationModel> webhook_table;
        public DeviceModel currDevice;
        public ObservableCollection<WebhookAutomationModel> WebhookTable
        {
            get { return webhook_table; }

            set
            {
                if (webhook_table != value)
                {
                    webhook_table = value;
                }
            }
        }

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

        public WebhookListViewModel(DeviceModel device)
        {
            this.currDevice = device;

            getwebhooks();
        }

        public void getwebhooks()
        {
            if (webhook_table == null)
            {
                WebhookTable = new ObservableCollection<WebhookAutomationModel>();
            }

            WebhookTable.Clear();

            var WebLst = WebhookService.getWebhooksforDevice(currDevice.Name);

            if (WebLst != null)
            {
                foreach (var webby in WebLst)
                {
                    WebhookTable.Add(webby);
                }
            }
        }

        public void RemoveWebhook(WebhookAutomationModel webby)
        {
            WebhookService.RemoveAutomation(currDevice.Name, webby);

            getwebhooks();
        }


    }
}
