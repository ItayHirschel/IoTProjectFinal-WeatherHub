using App1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		public LoginPage ()
		{
			InitializeComponent ();
			this.BindingContext = new LoginViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public async void OnLoginAttempt(object sender, EventArgs e)
        {
			LoginViewModel cont = (LoginViewModel)BindingContext;

			await cont.LoginButtonHandler();

            if (cont.isLoggedIn())
			{

                await App.NavigationService.NavigateToPageAsync(new ListViewPage1());
			}
        }

        public async void OnCreate(object sender, EventArgs e)
        {
            await App.NavigationService.NavigateToPageAsync(new CreateAccountPage());
        }
    }
}