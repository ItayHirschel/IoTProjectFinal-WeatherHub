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
    public partial class AddWebhookPage : ContentPage
    {
        public AddWebhookPage(DeviceModel model)
        {
            this.BindingContext = new NewWebhookViewModel(model);
            InitializeComponent();
        }

        public async void OnAddButton(object sender, EventArgs e)
        {
            ((NewWebhookViewModel)this.BindingContext).AddButtonHandler();
            DeviceModel model = ((NewWebhookViewModel)this.BindingContext).currDevice;
            await App.NavigationService.NavigateToPageAsync(new WebhookListPage(model));
        }

        public async void OnBack(object sender, EventArgs e)
        {
            DeviceModel model = ((NewWebhookViewModel)this.BindingContext).currDevice;
            await App.NavigationService.NavigateToPageAsync(new WebhookListPage(model));
        }
    }
}