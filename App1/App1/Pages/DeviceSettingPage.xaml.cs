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
    public partial class DeviceSettingPage : ContentPage
    {
        public DeviceSettingPage(DeviceModel model)
        {
            this.BindingContext = new DeviceSettingsViewModel(model);
            InitializeComponent();
        }

        public void OnBack(object sender, EventArgs e)
        {
            DeviceModel model = ((DeviceSettingsViewModel)BindingContext)._curr_device;
            App.NavigationService.NavigateToPageAsync(new DeviceMenuPage(model));
        }
    }
}