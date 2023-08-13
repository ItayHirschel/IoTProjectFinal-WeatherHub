using App1.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Essentials;

namespace App1.Services
{
    public class DeviceService
    {
        
        public static List<DeviceModel> Devices;
        private static string _device_detail_url = "https://iot-project-15989.azurewebsites.net/api/getdevicedetails";
        private static HttpClient client = new HttpClient();
        private static object _lock = new object();

        public static void init()
        {
            lock (_lock)
            {
                Devices = new List<DeviceModel>();
            }
        }

        public static List<string> getDeviceNames()
        {
            var list = new List<string>();

            lock (_lock)
            {
                foreach (var device in Devices)
                {
                    list.Add(device.FriendlyName);
                }
            }

            return list;
        }

        public static void Assign(List<DeviceModel> lst)
        {
            lock (_lock)
            {
                Devices = lst;
            }

        }

        public static void Reset()
        {
            lock (_lock)
            {
                Devices = new List<DeviceModel>();
            }

            WebhookService.updateFromDevService();
            WebhookService.SaveWebhookDB();
        }

        public static List<DeviceModel> getDevices()
        {
            List < DeviceModel > l = new List<DeviceModel>();
            lock (_lock)
            {
                foreach (var device in Devices)
                {
                    l.Add(device);
                }
            }
            return l;
        }

        public static DeviceModel getDevice(string name)
        {
            DeviceModel dev = null;
            lock(_lock)
            {
                foreach (var device in Devices)
                {
                    if (device.Name.Equals(name))
                    {
                        dev = device;
                        break;
                    }
                }
            }
            return dev;
        }

        public static void putDevice(DeviceModel dev)
        {
            
            lock (_lock)
            {
                if (getDevice(dev.Name) == null)
                {
                    Devices.Add(dev);
                }
            }
        }

       

        public static async void getDeviceDetails()
        {
            if (null == AccountDetails.key) return;

            var key = AccountDetails.key;
            
            foreach (var dev in Devices)
            {
                string dev_name = dev.Name;

                var responseMSG = await client.GetAsync(_device_detail_url + 
                    "?Account=" + AccountDetails.key + 
                    "&Device=" + dev_name);

                if ((responseMSG == null) || responseMSG.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    continue;
                }

                string msgstr = await responseMSG.Content.ReadAsStringAsync();

                string[] subs = msgstr.Split('{', '}', ',');
                lock(_lock)
                {
                    foreach (string sub in subs)
                    {
                        if (!sub.Contains(":")) continue;

                        string[] subsubstrings = sub.Split(':');
                        string subkey = subsubstrings[0].Trim().
                            Trim('"');
                        string subvalue = subsubstrings[1].Trim().
                            Trim('"');

                        if ("AutoSaveHours".Equals(subkey))
                        {
                            bool bool_temp = "true".Equals(subvalue.ToLower());
                            dev._auto_save_temperature = bool_temp;
                        }
                        else if ("AutoSaveTemp".Equals(subkey))
                        {
                            bool bool_temp = "true".Equals(subvalue.ToLower());
                            dev._auto_save_hours = bool_temp;
                        }
                        else if ("Location".Equals(subkey))
                        {
                            string str_temp;
                            if (!subvalue.Contains(";")) str_temp = null;
                            else
                            {
                                string[] coors = subvalue.Split(';');
                                str_temp = String.Join(",", coors);
                            }
                            dev._location = str_temp;
                        }
                        else if ("OffWebhook".Equals(subkey))
                        {
                            dev.offWebhook = subvalue;
                        }
                        else if ("OnWebhook".Equals(subkey))
                        {
                            dev.onWebhook = subvalue;
                        }
                        else if ("TurnOffHour".Equals(subkey))
                        {
                            dev._turn_off_hour = int.Parse(subvalue);
                        }
                        else if ("TurnOnHour".Equals(subkey))
                        {
                            dev._turn_on_hour = int.Parse(subvalue);
                        }
                        else if ("TurnOnTemp".Equals(subkey))
                        {
                            dev._turn_on_temp = int.Parse(subvalue);
                        }
                        else if ("TurnOffTemp".Equals(subkey))
                        {
                            dev._turn_off_temp = int.Parse(subvalue);
                        }
                    }
                }
            }
        }
    }


}
