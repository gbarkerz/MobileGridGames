using MobileGridGames.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnSettingsMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var settingsPage = new SettingsPage();

            await Navigation.PushModalAsync(settingsPage);
        }

        private async void OnHelpMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var answer = await DisplayAlert(
                "Help",
                "Would you like to visit the developer site which contains details on how to play this game?",
                "Yes", "No");
            if (answer)
            {
                Browser.OpenAsync("https://github.com/gbarkerz/MobileGridGames/blob/master/README.md");
            }
        }
    }
}
