using MobileGridGames.Services;
using MobileGridGames.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppSettingsPage : ContentPage
	{
		public AppSettingsPage ()
		{
			InitializeComponent ();

            this.BindingContext = new AppSettingsViewModel();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}