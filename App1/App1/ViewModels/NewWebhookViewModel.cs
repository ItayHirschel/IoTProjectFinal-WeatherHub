using App1.Models;
using App1.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace App1.ViewModels
{
    internal class NewWebhookViewModel : INotifyPropertyChanged
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

        private string _new_webhook;
        private string _f_name;
        public DeviceModel currDevice;

        public NewWebhookViewModel(DeviceModel dev)
        {
            this.currDevice = dev;
        }


        public string NewWebhook
        {
            get { return _new_webhook; }
            set { SetProperty(ref _new_webhook, value); }
        }

        public string NewFriendlyName
        {
            get { return _f_name; }
            set { SetProperty(ref _f_name, value); }
        }
        private string err_msg;
        public string ErrorMsg
        {
            get => err_msg;
            set { SetProperty(ref err_msg, value); }
        }

        public void AddButtonHandler()
        {
            if (null  == _new_webhook || null == _f_name) 
            {
                ErrorMsg = "Webhook or Webhook name is empty";
                return; 
            }
            if (_new_webhook.Length==0 || _f_name.Length == 0)
            {
                ErrorMsg = "Webhook or Webhook name is empty";
                return;
            }
            if (WebhookService.getWebhook(currDevice.Name, _f_name) != null)
            {
                ErrorMsg = "Name already in use";
                return;
            }

            WebhookService.AddAutomation(currDevice.Name, new WebhookAutomationModel(_f_name, _new_webhook));
        }
    }
}

