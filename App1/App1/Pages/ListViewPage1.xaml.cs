using App1.Models;
using App1.Pages;
using App1.Services;
using App1.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using System.Collections;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.Net.Mime.MediaTypeNames;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage1 : ContentPage
    {
        private static string url = "https://iot-project-15989.azurewebsites.net/api/";
        public ListViewPage1()
        {
            InitializeComponent();

            this.BindingContext = new DeviceListViewModel();
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var mi = e.Item as DeviceModel;
            await Navigation.PushModalAsync(new DeviceMenuPage(mi));
        }

        public void OnMore(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            Navigation.PushModalAsync(new DeviceMenuPage((DeviceModel)mi.CommandParameter));
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            string device_name = ((DeviceModel)mi.CommandParameter).Name;
            DeviceListViewModel model = (DeviceListViewModel)this.BindingContext;

            model.DeleteCommand(device_name);


        }

        public void OnLogOut(object sender, EventArgs e)
        {
            AccountDetails.username = String.Empty;
            AccountDetails.password = String.Empty;
            AccountDetails.key = String.Empty;

            DeviceService.Reset();

            Navigation.PushModalAsync(new LoginPage());
        }

        public void OnAddSensor(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new AddSensorPage());
        }



    }
}
