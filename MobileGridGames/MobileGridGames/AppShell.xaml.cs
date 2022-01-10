using MobileGridGames.ViewModels;
using MobileGridGames.Views;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            var settingsPage = new SettingsPage();

            await Navigation.PushModalAsync(settingsPage);

            Shell.Current.FlyoutIsPresented = false;
        }
    }
}
