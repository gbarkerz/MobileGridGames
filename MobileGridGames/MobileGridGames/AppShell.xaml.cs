using MobileGridGames.ViewModels;
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

            // Assume that the current page has to be the Squares page.
            var squaresPage = CurrentPage as SquaresPage;
            if (squaresPage != null)
            {
                var vm = squaresPage.BindingContext as SquaresViewModel;
                if (!vm.GameIsNotReady)
                {
                    var settingsPage = new SettingsPage();
                    await Navigation.PushModalAsync(settingsPage);
                }
            }
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

        private void OnRestartMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            // Assume that the current page has to be the Squares page.
            var squaresPage = CurrentPage as SquaresPage;
            if (squaresPage != null)
            {
                var vm = squaresPage.BindingContext as SquaresViewModel;
                vm.ResetGrid();
            }
        }
    }
}
