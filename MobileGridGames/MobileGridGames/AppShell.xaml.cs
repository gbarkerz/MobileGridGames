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
                Title = "Squares",
                FlyoutIcon = ImageSource.FromResource(
                    "MobileGridGames.Resources.SquaresGameIcon.png"),
            };

            shellSectionSquares.Items.Add(new ShellContent() { Content = new SquaresPage() });

            ShellSection shellSectionMatching = new ShellSection
            {
                Title = "Pairs",
                FlyoutIcon = ImageSource.FromResource(
                    "MobileGridGames.Resources.MatchingGameIcon.png"),
            };

            shellSectionMatching.Items.Add(new ShellContent() { Content = new MatchingPage() });

            bool initialGameIsSquares = (initialGame == "Squares");

            // The following attempt to bind the FlyoutBehavior to a view model property
            // seemed to work on startup, but not later when loading a different picture
            // into the Squares game. Future: Investigate this approach more thoroughly.
            //if (initialGameIsSquares)
            //{
            //    var page = shellSectionSquares.Items[0].Content as SquaresPage;
            //    var vm = page.BindingContext as BaseViewModel;

            //    this.BindingContext = vm;

            //    this.SetBinding(FlyoutBehaviorProperty,
            //        new Binding("GameIsLoading", BindingMode.OneWay,
            //            new GameIsLoadingToFlyoutBehavior()));
            //}

            this.Items.Insert(0, shellSectionMatching);
            this.Items.Insert(0, shellSectionSquares);

            this.CurrentItem = (initialGameIsSquares ? shellSectionSquares : shellSectionMatching);
        }

        private async void OnHelpMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var currentPage = this.CurrentPage;
            var helpPage = new HelpPage(currentPage is MatchingPage);
            await Navigation.PushModalAsync(helpPage);

            //var answer = await DisplayAlert(
            //    "Help",
            //    "Would you like to visit the developer site which contains details on how to play this game?",
            //    "Yes", "No");
            //if (answer)
            //{
            //    Browser.OpenAsync("https://github.com/gbarkerz/MobileGridGames/blob/master/README.md");
            //}
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
