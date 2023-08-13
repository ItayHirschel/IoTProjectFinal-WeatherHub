using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Xamarin.Forms;

namespace App1.Models
{
    public class DeviceModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }

            get => _name;
        }

        public string FriendlyName
        {
            set
            {
                _friendly_name = value;
                OnPropertyChanged("FriendlyName");
            }

            get => _friendly_name;
        }

        public string onWebhook;
        public string offWebhook;
        private string _friendly_name;

        public string _location;

        public bool _auto_save_hours;

        public bool _auto_save_temperature;

        public int _turn_on_hour;

        public int _turn_off_hour;

        public int _turn_on_temp;

        public int _turn_off_temp;


        public string Location
        {
            set
            {
                _location = value;
                OnPropertyChanged("Location");
            }

            get => _location;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DeviceModel(string name, string friendly_name)
        {
            Name = name;
            FriendlyName = friendly_name;
            Location = null;
        }

        public DeviceModel()
        {
            Name = "";
        }
        public string to_Json()
        {
            string content = "{" +
                "\"AutoSaveHours\":" + bool_2_string(_auto_save_hours) +
                ",\"AutoSaveTemp\":" + bool_2_string(_auto_save_temperature) +
                ",\"FriendlyName\":" + parenthesis(FriendlyName) +
                ",\"Location\":" + coor_2_json(Location) +
                ",\"OffWebhook\":" + parenthesis(offWebhook) +
                ",\"OnWebhook\":" + parenthesis(onWebhook) +
                ",\"TurnOffHour\":" + _turn_off_hour.ToString() +
                ",\"TurnOffTemp\":" + _turn_off_temp.ToString() +
                ",\"TurnOnHour\":" + _turn_on_hour.ToString() +
                ",\"TurnOnTemp\":" + _turn_on_temp.ToString() +
                "}";
            return content;
        }
        private string bool_2_string(bool value)
        {
            string val = "false";
            if (value) val = "true";
            return val;
        }

        private string coor_2_json(string coor)
        {
            if (null == coor) return "null";

            return parenthesis(String.Join(";", coor.Split(',')));
        }

        private string parenthesis(string s)
        {
            return "\"" + s + "\"";
        }
    }
}
