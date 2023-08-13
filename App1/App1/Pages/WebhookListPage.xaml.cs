using App1.Models;
using App1.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using App1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebhookListPage : ContentPage
    {

        public WebhookListPage(DeviceModel dev)
        {
            this.BindingContext = new WebhookListViewModel(dev);
            InitializeComponent();

        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            var mi = e.Item as WebhookAutomationModel;
            DeviceModel dev = ((WebhookListViewModel)BindingContext).currDevice;

            await WebhookService.ActivateWebhook(dev.Name, mi);

            await DisplayAlert("Webhook Activated", "The webhook " + mi.AutoName + ", " + mi.AutoWebhook + " was activated", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        public void OnBack(object sender, EventArgs e)
        {
            DeviceModel dev = ((WebhookListViewModel)BindingContext).currDevice;
            Navigation.PushModalAsync(new DeviceMenuPage(dev));
        }

        public void OnDelete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            WebhookAutomationModel webby = ((WebhookAutomationModel)mi.CommandParameter);
            DeviceModel dev = ((WebhookListViewModel)this.BindingContext).currDevice;
            WebhookService.RemoveAutomation(dev.Name, webby);
            ((WebhookListViewModel)this.BindingContext).getwebhooks();
            return;
        }

        public void OnAddWebhook(object sender, EventArgs e)
        {
            DeviceModel dev = ((WebhookListViewModel)BindingContext).currDevice;
            Navigation.PushModalAsync(new AddWebhookPage(dev));
        }
    }
}
