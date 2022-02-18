using MobileGridGames.ViewModels;
using MobileGridGames.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

// Future: Throughout this app, there's much explicit checking of view model 
// properties before taking action such as showing game settings or responding 
// to selections from the app flyout items. Replace all that explicit action
// by binding view model properties such that entry point UI is disabled, and
// so the various action of interest can't be triggered in the first place.

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
            if (currentPage is MatchingPage)
            {
                var vm = (CurrentPage as MatchingPage).BindingContext as MatchingViewModel;
                if (!vm.FirstRunMatching)
                {
                    await Navigation.PushModalAsync(new HelpPage(true));
                }
            }
            else
            {
                var vm = (CurrentPage as SquaresPage).BindingContext as SquaresViewModel;
                if (!vm.FirstRunSquares)
                {
                    await Navigation.PushModalAsync(new HelpPage(false));
                }
            }
        }

        private async void OnAppSettingsMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            bool showAppSettingWindow = true;

            var currentPage = this.CurrentPage;
            if (currentPage is MatchingPage)
            {
                var vm = (CurrentPage as MatchingPage).BindingContext as MatchingViewModel;
                showAppSettingWindow = !vm.FirstRunMatching;
            }
            else
            {
                var vm = (CurrentPage as SquaresPage).BindingContext as SquaresViewModel;
                showAppSettingWindow = !(vm.FirstRunSquares || vm.GameIsLoading);
            }

            if (showAppSettingWindow)
            {
                var appSettingsPage = new AppSettingsPage();
                await Navigation.PushModalAsync(appSettingsPage);
            }
        }

        private void OnRestartMenuItemClicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var currentPage = this.CurrentPage;
            if (currentPage is MatchingPage)
            {
                var vm = (CurrentPage as MatchingPage).BindingContext as MatchingViewModel;
                if (!vm.FirstRunMatching)
                {
                    vm.ResetGrid(true);
                }
            }
            else
            {
                var vm = (CurrentPage as SquaresPage).BindingContext as SquaresViewModel;
                if (!vm.FirstRunSquares && !vm.GameIsLoading)
                {
                    vm.ResetGrid();
                }
            }
        }
    }
}
