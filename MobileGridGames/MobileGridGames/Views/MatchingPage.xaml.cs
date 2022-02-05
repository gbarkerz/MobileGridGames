using MobileGridGames.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchingPage : ContentPage
    {
        private async void MatchingGameSettingsButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var settingsPage = new MatchingGameSettingsPage();
            await Navigation.PushModalAsync(settingsPage);
        }

        public MatchingPage()
        {
            InitializeComponent();
        }

        // This gets called when switching from the Squares Game to the Matching Game,
        // and also when closing the Matching Settings page.
        protected override void OnAppearing()
        {
            Debug.Write("Matching Game: OnAppearing called.");

            base.OnAppearing();

            Preferences.Set("InitialGame", "Matching");

            // Account for the app settings changing since the page was last shown.
            var vm = this.BindingContext as MatchingViewModel;
            vm.PlaySoundOnMatch = Preferences.Get("PlaySoundOnMatch", true);
            vm.PlaySoundOnNotMatch = Preferences.Get("PlaySoundOnNotMatch", true);
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            int itemIndex = (int)(e as TappedEventArgs).Parameter;

            Debug.WriteLine("Grid Games: Tapped on Square " + itemIndex);

            int itemCollectionIndex = GetItemCollectionIndexFromItemIndex(itemIndex);
            if (itemCollectionIndex == -1)
            {
                return;
            }

            var vm = this.BindingContext as MatchingViewModel;

            bool gameIsWon = vm.AttemptToTurnOverSquare(itemCollectionIndex);
            if (gameIsWon)
            {
                await OfferToRestartGame();
            }
        }

        private async Task OfferToRestartGame()
        {
            var vm = this.BindingContext as MatchingViewModel;

            var answer = await DisplayAlert(
                "Congratulations!",
                "You won the game in " +
                (8 + vm.TryAgainCount) +
                " goes.\r\n\r\nWould you like another game?",
                "Yes", "No");
            if (answer)
            {
                vm.ResetGrid();
            }
        }

        private int GetItemCollectionIndexFromItemIndex(int itemIndex)
        {
            var vm = this.BindingContext as MatchingViewModel;

            int itemCollectionIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (vm.SquareListCollection[i].Index == itemIndex)
                {
                    itemCollectionIndex = i;
                    break;
                }
            }

            return itemCollectionIndex;
        }

        private async void MatchingGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Matching Grid Game: Selection changed. Selection count is " + e.CurrentSelection.Count);

            // No action required here if there is no selected item.
            if (e.CurrentSelection.Count > 0)
            {
                var vm = this.BindingContext as MatchingViewModel;

                bool gameIsWon = vm.AttemptTurnUpBySelection(e.CurrentSelection[0]);

                // Clear the selection now to support the same square moving again.
                SquaresCollectionView.SelectedItem = null;

                if (gameIsWon)
                {
                    await OfferToRestartGame();
                }
            }
        }
    }
}
