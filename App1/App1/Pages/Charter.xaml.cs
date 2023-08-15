using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using App1.ViewModels;
using App1.Pages;

namespace App1
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Charter : ContentPage
    {
        public Charter(DeviceModel device)
        {
            this.BindingContext = new DeviceViewModel(device);
            InitializeComponent();
        }

        public void OnBack(object sender, EventArgs e)
        {
            DeviceModel dev = ((DeviceViewModel)BindingContext).curr_device;
            App.NavigationService.NavigateToPageAsync(new DeviceMenuPage(dev));
        }
    }
}