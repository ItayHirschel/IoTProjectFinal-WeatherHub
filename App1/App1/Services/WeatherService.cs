using App1.Interfaces;
using App1.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;


namespace App1.Services
{
    static class WeatherService
    {
        public struct Phrase
        {
            public string description;
            public int iconCode;
            public int Humdity;
        }
        private static readonly object prevPrecipitationLock = new object();
        private static readonly object CurrPhraseLock = new object();
        private static string apiUrl = "https://iot-project-15989.azurewebsites.net/api/getprecipitation";
        private static string phraseUrl = "https://iot-project-15989.azurewebsites.net/api/getphrase";
        private static Dictionary<string, bool> prevPrecipitation;
        private static Dictionary<string, Phrase> CurrPhrase;
        private static HttpClient client = new HttpClient();
        private static int freq = 1000 * 60 * 5;
        private static ILocalNotificationService localNotification;
        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";
        private static HubConnection connection;

        public static Phrase getCurrPhrase(string dev_name)
        {
            lock (CurrPhraseLock)
            {
                if (!CurrPhrase.ContainsKey(dev_name)) CurrPhrase[dev_name] = new Phrase { description = "No Information", iconCode = 0 , Humdity = 0 };
                return CurrPhrase[dev_name];
            }
        }

        public static void init()
        {
            prevPrecipitation = new Dictionary<string, bool>();
            CurrPhrase = new Dictionary<string, Phrase>();
            
            foreach (var dev in DeviceService.getDevices())
            {
                lock (prevPrecipitationLock) { prevPrecipitation[dev.Name] = false; }
                lock (CurrPhraseLock) { CurrPhrase[dev.Name] = new Phrase { description = "No Information", iconCode = 0 , Humdity = 0}; }
            }

            localNotification = DependencyService.Get<ILocalNotificationService>();
            connectToSignalR();
        }

        private static async void updatePrecipitation()
        {
            foreach (var dev in DeviceService.getDevices())
            {
                if (null == dev.Location) continue;

                var responseMSG = await client.GetAsync(apiUrl + "?coordinates=" + dev.Location);

                if (responseMSG == null) continue;

                if (responseMSG.StatusCode != System.Net.HttpStatusCode.OK) continue;

                var MSG = await responseMSG.Content.ReadAsStringAsync();

                lock (prevPrecipitationLock)
                {
                    if (!prevPrecipitation.ContainsKey(dev.Name))
                    {
                        bool state = "true".Equals(MSG.ToLower());
                        prevPrecipitation[dev.Name] = state;
                        continue;
                    }

                    if ("false".Equals(MSG.ToLower()))
                    {
                        if (prevPrecipitation[dev.Name])
                        {
                            localNotification.ShowNotification("Weather Update",
                                "Precipitation had stopped in the location of device : " + dev.FriendlyName,
                                new Dictionary<string, string>());

                        }

                        prevPrecipitation[dev.Name] = false;
                    }

                    else if ("true".Equals(MSG.ToLower()))
                    {
                        if (!prevPrecipitation[dev.Name])
                        {
                            localNotification.ShowNotification("Weather Update",
                                "Precipitation had started in the location of device : " + dev.FriendlyName,
                                new Dictionary<string, string>());
                        }

                        prevPrecipitation[dev.Name] = true;
                    }
                }
            }

        }

        private static async void updatePhrase()
        {
            foreach (var dev in DeviceService.getDevices())
            {
                if (null == dev.Location) continue;

                var responseMSG = await client.GetAsync(phraseUrl + "?coordinates=" + dev.Location);

                if (responseMSG == null) continue;

                if (responseMSG.StatusCode != System.Net.HttpStatusCode.OK) continue;

                var MSG = await responseMSG.Content.ReadAsStringAsync();

                lock (CurrPhraseLock) { CurrPhrase[dev.Name] = processPhrase(MSG); }
                
            }
        }

        public static void updateWeather()
        {
            updatePhrase();
            updatePrecipitation();
        }

        public static async void precipitationLoop()
        {
            while (true)
            {
                updatePhrase();
                updatePrecipitation();
                Thread.Sleep(freq);
            }
        }

        public static void RemoveDevice(string dev_name)
        {
            lock (prevPrecipitationLock)
            {
                if (prevPrecipitation.ContainsKey(dev_name))
                {
                    prevPrecipitation.Remove(dev_name);
                }
            }
            lock (CurrPhraseLock)
            {
                if (CurrPhrase.ContainsKey(dev_name))
                {
                    CurrPhrase.Remove(dev_name);
                }
            }
        }

        private static Phrase processPhrase(string s)
        {
            
            Phrase phrase = new Phrase { description = "No Information", iconCode = 0 , Humdity = 0};
            if (s == null) return phrase;
            
            string[] subs = s.Split(',');

            if (subs.Length == 3)
            {
                phrase.description = subs[0];
                phrase.iconCode = int.Parse(subs[1]);
                phrase.Humdity = int.Parse(subs[2]);
            }
            return phrase;
        }

        private static async void connectToSignalR()
        {
            var builder = new HubConnectionBuilder()
                .WithUrl(apiBaseUrl + "/api")
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                });

            connection = builder.Build();

            connection.On<string>("InformWeatherWarm", (message) =>
            {
                DeviceModel dev = DeviceService.getDevice(message);
                if (dev != null)
                {
                    localNotification.ShowNotification("Weather Update",
                                "Temperature near " + dev.FriendlyName + " is currently warm",
                                new Dictionary<string, string>());
                }
                
            });

            connection.On<string>("InformWeatherCold", (message) =>
            {
                DeviceModel dev = DeviceService.getDevice(message);
                if (dev != null)
                {
                    localNotification.ShowNotification("Weather Update",
                                "Temperature near " + dev.FriendlyName + " is currently cold",
                                new Dictionary<string, string>());
                }
            });


            try
            {
                await connection.StartAsync();

            }
            catch (Exception ex)
            { }
        }
    }
}
