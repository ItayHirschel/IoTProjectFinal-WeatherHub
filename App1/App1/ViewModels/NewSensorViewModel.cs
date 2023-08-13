using App1.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace App1.ViewModels
{
    class NewSensorViewModel : INotifyPropertyChanged
    {

        public NewSensorViewModel()
        {
            NewSensorName = string.Empty;
            NewFriendlyName = string.Empty;
            success = false;

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

        private string _new_sensor_name;
        private string _f_name;
        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";
        private static HttpClient client = new HttpClient();
        private string _err;
        public bool success;

        public string Err
        {
            get => _err;
            set => SetProperty(ref _err, value);
        }

        public string NewSensorName
        {
            get { return _new_sensor_name; }
            set { SetProperty(ref _new_sensor_name, value); }
        }

        public string NewFriendlyName
        {
            get { return _f_name; }
            set { SetProperty(ref _f_name, value); }
        }

        public async Task AddButtonHandler()
        {
            success = false;
            string add_url = apiBaseUrl + "/api/addUser?newUser=" + NewSensorName + 
                "&FriendlyName=" + NewFriendlyName +
                "&Account=" + AccountDetails.key;
            var response = await client.GetAsync(add_url);
            if (response == null) return;
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Err = await response.Content.ReadAsStringAsync();
                success = false;
                return;
            }
            success = true;
            return;

        }
    }
}
