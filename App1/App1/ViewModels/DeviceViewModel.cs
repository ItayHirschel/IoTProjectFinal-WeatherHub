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
using Microcharts;
using SkiaSharp;
using System.Transactions;
using App1.Models;
using App1.Services;
using static App1.Services.WeatherService;

namespace App1.ViewModels
{
    public class DeviceViewModel : INotifyPropertyChanged
    {
        public DeviceModel curr_device;
        private static object chartlock = new object();
        public static ObservableCollection<SensorData> table;

        private string _phrase;
        private int iconCode;

        public int IconCode
        {
            get => iconCode;

            set
            {
                SetProperty(ref iconCode, value);
                OnPropertyChanged("ImageSource");
            }
        }
        public string Phrase
        {
            get => _phrase;
            set
            {
                SetProperty(ref _phrase, value);
            }
        }

        private int _humidity;
        public int Humidity
        {
            get => _humidity;

            set
            {
                SetProperty(ref _humidity, value);
            }
        }

        private static string url = "https://iot-project-15989.azurewebsites.net/api/get_values";

        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";

        private static HttpClient client = new HttpClient();

        private static int N = 10;

        private HubConnection connection;

        private string partition;

        private LineChart tempchart;
        private LineChart preschart;



        public LineChart TempChart
        {
            set
            {
                if (tempchart != value)
                {
                    tempchart = value;
                    OnPropertyChanged("TempChart");
                }
            }
            get { return tempchart; }
        }

        public LineChart PresChart
        {
            set
            {
                if (preschart != value)
                {
                    preschart = value;
                    OnPropertyChanged("PresChart");
                }
            }
            get { return preschart; }
        }

        public string ImageSource
        {
            get => $"weather_icon_{iconCode}.png";
        } 


        private string device_fname;

        public string Device_fname
        {
            get { return device_fname; }
            set { SetProperty(ref device_fname, value); }
        }
        public string Partition
        {
            get { return partition; }
            set { SetProperty(ref partition, value); }
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

        public ObservableCollection<SensorData> Table
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

        
        public DeviceViewModel(DeviceModel device)
        {
            WeatherService.updateWeather();
            curr_device = device;
            Partition = device.Name;
            Device_fname = device.FriendlyName;
            Table = new ObservableCollection<SensorData>();

            for (int i = 0; i < N; i++) { Table.Add(new SensorData()); }

            getCurrValues();

            connectToSignalR();

            keep_updates();

            TempChart = new LineChart { LineMode = LineMode.Straight, LabelTextSize = 40f, IsAnimated = false };
            PresChart = new LineChart { LineMode = LineMode.Straight, LabelTextSize = 40f, IsAnimated = false };

            WeatherService.Phrase phrase = WeatherService.getCurrPhrase(device.Name);

            Phrase = phrase.description;
            iconCode = phrase.iconCode;
            Humidity = phrase.Humdity;

            keep_updates_phrase();

        }





        public async void getCurrValues()
        {

            var responseMSG = await client.GetAsync(url + "?User=" + partition);

            if (null != responseMSG)
            {

                var jsonString = await responseMSG.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<List<Dictionary<string, double>>>(jsonString);

                lock (chartlock)
                {
                    for (int i = 0; i < N; i++)
                    {
                        int j = N - 1 - i;
                        Table[j].Pressure = deserialized[i]["PRES"];
                        Table[j].Temperature = deserialized[i]["TEMP"];
                        Table[j].Humidity = deserialized[i]["HUMID"];
                    }
                }


            }

            UpdateChart();
        }



        private async void keep_updates()
        {
            while (true)
            {
                getCurrValues();
                await Task.Delay(1000 * 60 * 15);

            }
        }

        private async void keep_updates_phrase()
        {
            while (true)
            {
                WeatherService.Phrase ph = WeatherService.getCurrPhrase(curr_device.Name);
                this.Phrase = ph.description;
                this.IconCode = ph.iconCode;
                this.Humidity = ph.Humdity;
                await Task.Delay(1000 * 10);

            }
        }


        private async void connectToSignalR()
        {
            var builder = new HubConnectionBuilder()
                .WithUrl(apiBaseUrl + "/api")
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                });

            connection = builder.Build();

            connection.On<string>("SensorUpdate", (message) => 
            {
                if (message.Equals(curr_device.Name)) getCurrValues();
            });


            try
            {
                await connection.StartAsync();

            }
            catch (Exception ex)
            { }
        }

        public void UpdateChart()
        {

            var new_temp = new ChartEntry[Table.Count];
            var new_pres = new ChartEntry[Table.Count];

            var values = new SensorData[Table.Count];
            int n = Table.Count;

            lock (chartlock)
            {
                for (int i = 0; i < Table.Count; i++)
                {
                    values[i] = Table[i];
                }
            }


            float mintemp = float.PositiveInfinity;
            float minpres = float.PositiveInfinity;


            foreach (var table in values)
            {
                if (table.Temperature < mintemp)
                {
                    mintemp = (float)table.Temperature;
                }
                if (table.Pressure < minpres)
                {
                    minpres = (float)table.Pressure;
                }
            }


            for (int i = 0; i < n; i++)
            {
                int j = Table.Count - 1 - i;
                new_temp[j] = new ChartEntry((float)values[i].Temperature - mintemp)
                {
                    Color = SKColor.Parse("ff80ff"),
                    ValueLabel = $"{values[i].Temperature}",
                    Label = $"{i} hr",
                    TextColor = SKColors.Black
                };

                new_pres[j] = new ChartEntry((float)values[i].Pressure - minpres)
                {
                    Color = SKColor.Parse("ff80ff"),
                    ValueLabel = $"{values[i].Pressure}",
                    Label = $"{i} hr",
                    TextColor = SKColors.Black
                };
            }



            lock (chartlock)
            {
                TempChart.Entries = new List<ChartEntry>(new_temp);
                PresChart.Entries = new List<ChartEntry>(new_pres);
            }


        }
    }

}
