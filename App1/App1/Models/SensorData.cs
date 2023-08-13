using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace App1.Models
{
    public class SensorData : INotifyPropertyChanged
    {
        private double _temperature;
        private double _pressure;
        private double _humidity;
        public double Temperature
        {
            set
            {
                _temperature = value;
                OnPropertyChanged("Temperature");
            }

            get => _temperature;
        }

        public double Pressure
        {
            set
            {
                _pressure = value;
                OnPropertyChanged("Pressure");
            }

            get => _pressure;
        }

        public double Humidity
        {
            set
            {
                _humidity = value;
                OnPropertyChanged("Humidity");
            }

            get => _humidity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
