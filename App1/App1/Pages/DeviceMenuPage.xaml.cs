using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceMenuPage : ContentPage
    {
        public DeviceMenuPage(DeviceModel dev)
        {
            this.BindingContext = new DeviceMenuViewModel(dev);
            InitializeComponent();
        }

        public void OnBack(object sender, EventArgs e)
        {
            App.NavigationService.NavigateToPageAsync(new ListViewPage1());
        }

        public void OnSettings(object sender, EventArgs e)
        {
            DeviceModel model = ((DeviceMenuViewModel)BindingContext).CurrDevice;
            App.NavigationService.NavigateToPageAsync(new DeviceSettingPage(model));
        }

        public void OnMeasurements(object sender, EventArgs e)
        {
            DeviceModel model = ((DeviceMenuViewModel)BindingContext).CurrDevice;
            App.NavigationService.NavigateToPageAsync(new Charter(model));
        }

        public void OnAutomation(object sender, EventArgs e)
        {
            DeviceModel model = ((DeviceMenuViewModel)BindingContext).CurrDevice;
            App.NavigationService.NavigateToPageAsync(new WebhookListPage(model));
        }
    }
}