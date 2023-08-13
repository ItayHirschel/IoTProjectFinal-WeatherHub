using App1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Net.Http;
using Xamarin.Essentials;
using App1.Services;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App1.ViewModels
{
    internal class DeviceSettingsViewModel : INotifyPropertyChanged
    {
        private const string SetSettingsUrl = "https://iot-project-15989.azurewebsites.net/api/SetDeviceDetails";
        private const string getCoorsUrl = "https://iot-project-15989.azurewebsites.net/api/GetCoordinates";
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

        public ICommand SetSettingsCommand { get; set; }

        private string _friendly_name;
        public string FriendlyName
        {
            get => _friendly_name;
            set
            {
                SetProperty(ref _friendly_name, value);
            }
        }

        private string _off_webhook;
        public string OffWebhook
        {
            get => _off_webhook;
            set
            {
                SetProperty(ref _off_webhook, value);
            }
        }
        private string _on_webhook;
        public string OnWebhook
        {
            get => _on_webhook;
            set
            {
                SetProperty(ref _on_webhook, value);
            }
        }
        private string _latitude;
        public string Latitude
        {
            get => _latitude;
            set
            {
                double l;
                if (double.TryParse(value, out l))
                {
                    SetProperty(ref _latitude, value);
                    NumericalErrLat = "";
                }
                else
                {
                    NumericalErrLat = "must contain number";
                }
            }
        }
        private string _numerical_err_lat;
        public string NumericalErrLat
        {
            get => _numerical_err_lat;
            set
            {
                SetProperty(ref _numerical_err_lat, value);
            }
        }
        private string _longitude;
        public string Longtitude
        {
            get => _longitude;
            set
            {
                double l;
                if (double.TryParse(value, out l))
                {
                    SetProperty(ref _longitude, value);
                    NumericalErrLot = "";
                }
                else
                {
                    NumericalErrLot = "must contain number";
                }
            }
        }
        private string _numerical_err_lot;
        public string NumericalErrLot
        {
            get => _numerical_err_lot;
            set
            {
                SetProperty(ref _numerical_err_lot, value);
            }
        }

        private int _turn_off_hour;
        public string TurnOffHour
        {
            get => _turn_off_hour.ToString();
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i >= 24) { OffHourErr = "hour value must be at most 23"; }
                    else
                    {
                        OffHourErr = "";
                        _turn_off_hour = i;
                    }
                }
                else
                {
                    OffHourErr = "must be integer";
                }

            }
        }
        private string _off_hour_err;
        public string OffHourErr
        {
            get => _off_hour_err;
            set
            {
                SetProperty(ref _off_hour_err, value);
            }
        }

        private int _turn_on_hour;

        public string TurnOnHour
        {
            get => _turn_on_hour.ToString();
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    if (i >= 24) { OnHourErr = "hour value must be at most 23"; }
                    else
                    {
                        OnHourErr = "";
                        _turn_on_hour = i;
                    }
                }
                else
                {
                    OnHourErr = "must be integer";
                }

            }
        }

        private string _on_hour_err;
        public string OnHourErr
        {
            get => _on_hour_err;
            set
            {
                SetProperty(ref _on_hour_err, value);
            }
        }

        private int _turn_off_temp;
        public string TurnOffTemp
        {
            get => _turn_off_temp.ToString();
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    OffTempErr = "";
                   _turn_off_temp = i;
                }
                else
                {
                    OffTempErr = "must be integer";
                }

            }
        }

        private string _off_temp_err;
        public string OffTempErr
        {
            get => _off_temp_err;
            set
            {
                SetProperty(ref _off_temp_err, value);
            }
        }

        private int _turn_on_temp;
        public string TurnOnTemp
        {
            get => _turn_on_temp.ToString();
            set
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    OnTempErr = "";
                    _turn_on_temp = i;
                }
                else
                {
                    OnTempErr = "must be integer";
                }

            }
        }

        private string _on_temp_err;
        public string OnTempErr
        {
            get => _on_temp_err;
            set
            {
                SetProperty(ref _on_temp_err, value);
            }
        }


        private bool _auto_save_temp;
        public bool AutoSaveTemp
        {
            get => _auto_save_temp;
            set
            {
                SetProperty(ref _auto_save_temp, value);
            }
        }
        public bool _auto_save_hour;
        public bool AutoSaveHour
        {
            get => _auto_save_hour;
            set
            {
                SetProperty(ref _auto_save_hour, value);
            }
        }

        private string _save_err;
        public string SaveErrors
        {
            get => _save_err;
            set
            {
                SetProperty(ref _save_err, value);
            }
        }

        private string _address;
        public string Address
        {
            get => _address;
            set
            {
                SetProperty(ref _address, value);
            }
        }
        private string _city;
        public string City
        {
            get => _city;
            set
            {
                SetProperty(ref _city, value);
            }
        }
        private string _country;
        public string Country
        {
            get => _country;
            set
            {
                SetProperty(ref _country, value);
            }
        }
        private string _LocError;
        public string LocError
        {
            get => _LocError;
            set
            {
                SetProperty(ref _LocError, value);
            }
        }

        private bool CheckAddress()
        {
            LocError = "";
            bool status = false;
            if (null == _address || _address.Length == 0)
            {
                status = true;
                LocError += "Address, ";
            }
            if (null == _city || _city.Length == 0)
            {
                status = true;
                LocError += "City, ";
            }
            if (null == _country || _country.Length == 0)
            {
                status = true;
                LocError += "Country, ";
            }

            if (status)
            {
                LocError += "are empty, can't process location request";
            }

            return status;
        }

        public async void GetCoors()
        {
            if (CheckAddress()) return;

            var client = new HttpClient();

            string query = string.Join(", ", new string[] {_address, _city, _country});

            var responseMSG = await client.GetAsync(getCoorsUrl + "?query=" + query);

            string msg = await responseMSG.Content.ReadAsStringAsync();

            string[] subs = msg.Split(',');
            if (subs.Length == 2)
            {
                Latitude = subs[0];
                Longtitude = subs[1];
            }
            else
            {
                _latitude = "";
                _longitude = "";
                LocError = "ther recieved message \'" + msg + "\' couldn't be precessed to corrdinates";
            }
        }

        public DeviceModel _curr_device;

        public ICommand getCoorsCommand { get; set; }
        public DeviceSettingsViewModel(DeviceModel model)
        {
            FriendlyName = model.FriendlyName;
            OffWebhook = model.offWebhook;
            OnWebhook = model.onWebhook;
            _turn_off_hour = model._turn_off_hour;
            _turn_on_hour = model._turn_on_hour;
            _turn_off_temp = model._turn_off_temp;
            _turn_on_temp = model._turn_on_temp;
            _auto_save_hour = model._auto_save_hours;
            _auto_save_temp = model._auto_save_temperature;
            if (null == model.Location)
            {
                model.Location = string.Empty;
            }
            string[] subs = model.Location.Split(',');
            if (subs.Length == 2 ) 
            {
                Latitude = subs[0];
                Longtitude = subs[1];
            }
            else
            {
                _latitude = "";
                _longitude = "";
            }
            _curr_device = model;

            SetSettingsCommand = new Command(SaveSettings);
            getCoorsCommand = new Command(GetCoors);
        }

        public async void SaveSettings()
        {
            if (!CheckValidValues())
            {
                SaveErrors = "Invalid Values, Please check";
                return;
            }

            

            SaveSettingsToDevice();
            var content = new StringContent(_curr_device.to_Json());
            var client = new HttpClient();
            var responseMSG = await client.PostAsync(SetSettingsUrl + 
                "?Account=" + AccountDetails.key +
                "&Device=" + _curr_device.Name,
                content);
            if (responseMSG == null) return;
            if (responseMSG.StatusCode != System.Net.HttpStatusCode.OK)
            {
                SaveErrors = await responseMSG.Content.ReadAsStringAsync();
            }
            else
            {
                SaveErrors = "Settings Saved";
            }
            WeatherService.updateWeather();
        }

        private bool CheckValidValues()
        {
            if (_turn_off_hour > 23 || _turn_on_hour > 23) return false;

            if (Latitude.Equals(""))
            {
                if (Longtitude.Equals(""))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (Longtitude.Equals(""))
                {
                    return false;
                }

                double l;
                if (!double.TryParse(_longitude, out l) || !double.TryParse(_latitude, out l))
                { 
                    return false;
                }
            }

            return true;
        }
        private void SaveSettingsToDevice()
        { 
            var model = _curr_device;
            model.FriendlyName = _friendly_name;
            model.offWebhook = _off_webhook;
            model.onWebhook = _on_webhook;
            model._turn_off_hour = _turn_off_hour;
            model._turn_on_hour = _turn_on_hour;
            model._turn_off_temp = _turn_off_temp;
            model._turn_on_temp = _turn_on_temp;
            model._auto_save_hours = _auto_save_hour;
            model._auto_save_temperature = _auto_save_temp;
            model.Location = Latitude + "," + Longtitude;

        }

    }
}
