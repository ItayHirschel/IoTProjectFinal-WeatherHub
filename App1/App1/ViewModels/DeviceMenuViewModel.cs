using App1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace App1.ViewModels
{
    internal class DeviceMenuViewModel : INotifyPropertyChanged
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

        public DeviceModel CurrDevice;
        private string _dev_f_name;

        public string DevFName
        {
            get => _dev_f_name;
            set
            {
                SetProperty(ref _dev_f_name, value);
            }
        }

        public DeviceMenuViewModel(DeviceModel device)
        {
            CurrDevice = device;
            DevFName = device.FriendlyName;
        }

    }
}
