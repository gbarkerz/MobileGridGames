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

            string initialGame = Preferences.Get("InitialGame", "Squares");

            ShellSection shellSectionSquares = new ShellSection
            {
                Title = "Squares Game V1.2",
                FlyoutIcon = ImageSource.FromResource(
                    "MobileGridGames.Resources.SquaresGameIcon.png"),
            };

            shellSectionSquares.Items.Add(new ShellContent() { Content = new SquaresPage() });

            ShellSection shellSectionMatching = new ShellSection
            {
                Title = "Matching Game V1.0",
                FlyoutIcon = ImageSource.FromResource(
                    "MobileGridGames.Resources.MatchingGameIcon.png"),
            };

            shellSectionMatching.Items.Add(new ShellContent() { Content = new MatchingPage() });

            // Future: This approach has the Squares Game's OnAppearing() called
            // even when the current game is the Matching Game. So rearrange things 
            // such that only the current game's OnAppearing() is called on startup.

            this.Items.Insert(0, shellSectionMatching);
            this.Items.Insert(0, shellSectionSquares);

            int currentIndex = (initialGame == "Squares" ? 0 : 1);
            this.CurrentItem = this.Items[currentIndex];
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

            var currentPage = this.CurrentPage;
            if (currentPage is MatchingPage)
            {
                var vm = (CurrentPage as MatchingPage).BindingContext as MatchingViewModel;
                vm.ResetGrid();
            }
            else
            {
                var vm = (CurrentPage as SquaresPage).BindingContext as SquaresViewModel;
                vm.ResetGrid();
            }
        }
    }
}
