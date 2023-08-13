using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using App1.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace App1.Services
{
    internal class WebhookService
    {
        private static string _file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "automations.json");
        private static string _webhook_url = "https://iot-project-15989.azurewebsites.net/api/sendwebhooktodevice";
        private static Dictionary<string, List<WebhookAutomationModel>> WebhookDB;
        private static HttpClient client = new HttpClient();
        private static object _lock = new object();

        public static void Initialize()
        {
            WebhookDB = new Dictionary<string, List<WebhookAutomationModel>>();
            updateFromFile();
            SaveWebhookDB();
        }
        public static void updateFromFile()
        {
            if (!File.Exists(_file_name))
            {
                File.Create(_file_name).Close();
                return;
            }
            lock (_lock)
            {
                WebhookDB.Clear();

                string json_string = File.ReadAllText(_file_name);

                var deserialized = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(json_string);

                if (null == deserialized)
                {
                    return;
                }

                foreach (var dev_name in deserialized.Keys)
                {
                    WebhookDB[dev_name] = new List<WebhookAutomationModel>();

                    List<Dictionary<string, string>> autos = deserialized[dev_name];

                    foreach (var auto in autos)
                    {
                        WebhookAutomationModel webby = new WebhookAutomationModel(auto["AutoName"], auto["AutoWebhook"]);
                        WebhookDB[dev_name].Add(webby);
                    }
                }
            }
        }

        public static void updateFromDevService()
        {
            lock (_lock)
            {
                foreach (DeviceModel moddy in DeviceService.getDevices())
                {
                    if (!WebhookDB.ContainsKey(moddy.Name))
                    {
                        WebhookDB[moddy.Name] = new List<WebhookAutomationModel>();
                    }
                }
            }
        }

        public static void SaveWebhookDB()
        {
            Dictionary<string, List<Dictionary<string, string>>> deserialized = new Dictionary<string, List<Dictionary<string, string>>>();
            lock (_lock)
            {
                foreach (string key in WebhookDB.Keys)
                {
                    deserialized[key] = new List<Dictionary<string, string>>();

                    foreach (var webby in WebhookDB[key])
                    {
                        Dictionary<string, string> d = new Dictionary<string, string>();
                        d["AutoName"] = webby.AutoName;
                        d["AutoWebhook"] = webby.AutoWebhook;
                        deserialized[key].Add(d);
                    }
                }

                var json_string = JsonConvert.SerializeObject(deserialized);


                File.WriteAllText(_file_name, json_string);
            }
        }

        public static void RemoveAutomation(string dev_name, WebhookAutomationModel webby)
        {
            lock(_lock)
            {
                if (WebhookDB.ContainsKey(dev_name))
                {
                    WebhookDB[dev_name].Remove(webby);
                }
            }
            SaveWebhookDB();
        }

        public static void AddAutomation(string dev_name, WebhookAutomationModel webby)
        {
            lock (_lock)
            {
                if (WebhookDB.ContainsKey(dev_name))
                {
                    WebhookDB[dev_name].Add(webby);
                }
            }
            SaveWebhookDB();
        }

        public static void RemoveDevice(string dev_name)
        {
            lock (_lock)
            {
                if (WebhookDB.ContainsKey(dev_name))
                {
                    WebhookDB.Remove(dev_name);
                }
            }
            SaveWebhookDB();
        }

        public static List<WebhookAutomationModel> getWebhooksforDevice(string dev_name)
        {
            List<WebhookAutomationModel> l = null;
            lock (_lock)
            {
                if (WebhookDB.ContainsKey(dev_name))
                {
                    l = WebhookDB[dev_name];
                }
            }
            return l;
        }

        public static WebhookAutomationModel getWebhook(string dev_name,  string webhook_name)
        {
            WebhookAutomationModel webby = null;
            lock (_lock)
            {
                if (WebhookDB.ContainsKey(dev_name))
                {
                    foreach (var w in WebhookDB[dev_name])
                    {
                        if (w.AutoName.Equals(webhook_name))
                        {
                            webby = w;
                            break;
                        }
                    }
                }
            }
            return webby;
        }

        public static async Task ActivateWebhook(string dev_name, WebhookAutomationModel webby)
        {

            await client.PostAsync(_webhook_url +
                "?device=" + dev_name +
                "&webhook=" + webby.AutoWebhook, null);
        }
    }
}
