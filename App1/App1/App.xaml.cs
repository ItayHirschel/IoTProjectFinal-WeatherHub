
using System;
using System.Linq;
using System.Threading.Tasks;
using App1.Models;
using App1.Pages;
using App1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1
{
    public partial class App : Application
    {
        public static NavigationService NavigationService { get; } = new NavigationService();

        public App()
        {

            InitializeComponent();
            DeviceService.init();
            WeatherService.init();
            Task.Run(async () => WeatherService.precipitationLoop());
            WebhookService.Initialize();
            MainPage = new NavigationPage(new LoginPage());
        }

        



        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
