using App1.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.ViewModels
{
    class CreateAccountViewModel : INotifyPropertyChanged
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

        private string _username;
        private string _password_1;
        private string _password_2;
        private string _err_msg;
        public bool create_success = false;
        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";
        private static HttpClient client = new HttpClient();

        public CreateAccountViewModel()
        {
            _username = string.Empty;
            _password_1 = string.Empty;
            _password_2 = string.Empty;
            _err_msg = string.Empty;
            create_success = false;
        }
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string ErrorMSG
        {
            get { return _err_msg; }
            set { SetProperty(ref _err_msg, value);  }
        }

        public string Password1
        {
            get { return _password_1; }
            set { SetProperty(ref _password_1, value); }
        }

        public string Password2
        {
            get { return _password_2; }
            set { SetProperty(ref _password_2, value); }
        }

        public async Task CreateButtonHandler()
        {
            if (!Password1.Equals(Password2))
            {
                ErrorMSG = "Please make sure the two passwords match";
                return;
            }
            //login_success = false;
            string login_url = apiBaseUrl + "/api/CreateNewAccount?AccountName=" + Username + "&Password=" + Password1;
            ErrorMSG = "Processing request, please wait.";
            var responseMSG = await client.GetAsync(login_url);

            if (responseMSG != null)
            {

                if (responseMSG.StatusCode != System.Net.HttpStatusCode.OK)
                {

                    ErrorMSG = await responseMSG.Content.ReadAsStringAsync();

                    create_success = false;
                    return;
                }

                string key = await responseMSG.Content.ReadAsStringAsync();



                create_success = true;
                return;
            }

            create_success = false;
            return;
        }
    }
}

