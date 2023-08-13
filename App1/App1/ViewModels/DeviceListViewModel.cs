using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using App1.Models;
using App1.Services;

namespace App1.ViewModels
{
    public class DeviceListViewModel : INotifyPropertyChanged
    {
        public static ObservableCollection<DeviceModel> table;

        private static string url = "https://iot-project-15989.azurewebsites.net/api/";

        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";

        private static HttpClient client = new HttpClient();

        private static int N = 10;

        private HubConnection connection;

        public bool buttonIsOn = false;
        private string _text;






        public bool ButtonIsOn
        {
            get => buttonIsOn;
            set => SetProperty(ref buttonIsOn, value);
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



        public async void DeleteCommand(string device_name)
        {

            string delete_url = url + "deleteUser?newUser=" + device_name + "&Account=" + AccountDetails.key;
            await client.GetAsync(delete_url);
            WeatherService.RemoveDevice(device_name);
            WebhookService.RemoveDevice(device_name);
            getCurrUsers();
        }
/*
        
*/
        public ICommand AddButtonCommand { get; }
        public ICommand DelButtonCommand { get; }

        public ObservableCollection<DeviceModel> Table
        {
            get { return table; }

            set
            {
                if (table != value)
                {
                    table = value;
                }
            }
        }

        public DeviceListViewModel()
        {

            Table = new ObservableCollection<DeviceModel>();

            getCurrUsers();

        }

        public async void getCurrUsers()
        {

            var content = new StringContent("", Encoding.UTF8);
            var responseMSG = await client.GetAsync(url + "getallusers?Account=" + AccountDetails.key);

            if (null != responseMSG)
            {
                Table.Clear();
                
                var jsonString = await responseMSG.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(jsonString);
                for (int i = 0; i < deserialized.Count; i++)
                {
                    string dev_serial = deserialized[i]["Serial"];
                    string dev_name = deserialized[i]["Name"];
                    var service_dev = DeviceService.getDevice(dev_serial);
                    if (null != service_dev)
                    {
                        Table.Add(service_dev);
                        continue;
                    }
                    var par = new DeviceModel();
                    par.Name = dev_serial;
                    par.FriendlyName = dev_name;
                    Table.Add(par);
                }
                DeviceService.Assign(new List<DeviceModel>(Table));
            }
            DeviceService.getDeviceDetails();
            WebhookService.updateFromDevService();
            WeatherService.updateWeather();
            WebhookService.SaveWebhookDB();
        }
    }

}
