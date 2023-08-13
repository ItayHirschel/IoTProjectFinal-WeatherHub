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
    class LoginViewModel : INotifyPropertyChanged
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
        private string _password;
        private string _err_msg;
        private bool login_success = false;
        private static string apiBaseUrl = "https://iot-project-15989.azurewebsites.net";
        private static HttpClient client = new HttpClient();

        public LoginViewModel()
        {
            _username = string.Empty;
            _password = string.Empty;
            _err_msg = string.Empty;
            login_success = false;
        }
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        public string ErrorMSG
        {
            get { return _err_msg; }
            set { SetProperty(ref _err_msg, value); }
        }

        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public async Task LoginButtonHandler()
        {
            //login_success = false;
            string login_url = apiBaseUrl + "/api/loginrequest?AccountName=" + Username + "&Password=" + Password;
            ErrorMSG = "Connecting, please wait.";
            var responseMSG = await client.GetAsync(login_url);
            
            if (responseMSG != null)
            {

                if (responseMSG.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    
                    ErrorMSG = await responseMSG.Content.ReadAsStringAsync();

                    login_success = false;
                    return;
                }

                string key = await responseMSG.Content.ReadAsStringAsync();
                AccountDetails.username = Username;
                AccountDetails.password = Password;
                AccountDetails.key = key;

                

                login_success = true;
                return;
            }

            login_success = false;
            return;
        }

        public bool isLoggedIn()
        {
            return login_success;
        }
    }
}
