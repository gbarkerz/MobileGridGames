using MobileGridGames.ViewModels;
using System;
using Xamarin.Essentials;
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

            var vm = this.BindingContext as AppSettingsViewModel;
            vm.ShowDarkTheme = Preferences.Get("ShowDarkTheme", false);
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();

            // Create a new AppShell here to force the new theme colours to be shown
            // on the hamburger button.

            // Future: This seems a little heavy-handed just to get the colours to refresh.
            // Investigate whether there's a simpler way to achieve the results.
            App.Current.MainPage = new AppShell();
        }
    }
}