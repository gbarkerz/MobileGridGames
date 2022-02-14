using MobileGridGames.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchingPage : ContentPage
    {
        private bool previousShowCustomPictures;
        private string previousPicturePath;

        public MatchingPage()
        {
            InitializeComponent();
        }

        private async void MatchingGameSettingsButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;

            var settingsPage = new MatchingGameSettingsPage();
            await Navigation.PushModalAsync(settingsPage);
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

            // Has something changed related to custom picture use since the last time
            // we were in OnAppearing()?
            var showCustomPictures = Preferences.Get("ShowCustomPictures", false);
            var picturePath = Preferences.Get("PicturePath", "");

            if ((showCustomPictures != previousShowCustomPictures) ||
                (picturePath != previousPicturePath))
            {
                // Reset all cached game progress setting, but don't bother to shuffle.
                vm.ResetGrid(false);

                // Should we be showing the default pictures?
                if (!showCustomPictures || String.IsNullOrWhiteSpace(picturePath))
                {
                    vm.SetupDefaultMatchingCardList();
                }
                else
                {
                    // Attempt to load up the custom pictures and associated accessible data.
                    var customPictures = new Collection<Card>();

                    bool resetToUseDefaultPictures = false;

                    // We should have 8 pairs of cards.
                    for (int i = 0; i < 8; i++)
                    {
                        // For each of the 2 cards in each pair...
                        for (int j = 0; j < 2; j++)
                        {
                            var card = new Card();

                            // The index is from 1-16.
                            card.Index = (i * 2) + j + 1;

                            string settingName = "Card" + (i + 1) + "Path";
                            var cardPath = Preferences.Get(settingName, "");
                            if (!File.Exists(cardPath))
                            {
                                Debug.WriteLine("Pairs: Card path missing.");

                                resetToUseDefaultPictures = true;

                                break;
                            }

                            try
                            {
                                card.PictureImageSource = ImageSource.FromFile(cardPath);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Pairs: Failed to load image. " + ex.Message);

                                resetToUseDefaultPictures = true;

                                break;
                            }

                            settingName = "Card" + (i + 1) + "Name";
                            card.AccessibleName = Preferences.Get(settingName, "");
                            if (String.IsNullOrWhiteSpace(card.AccessibleName))
                            {
                                Debug.WriteLine("Pairs: Accessible name missing.");

                                resetToUseDefaultPictures = true;

                                break;
                            }

                            settingName = "Card" + (i + 1) + "Description";
                            card.AccessibleDescription = Preferences.Get(settingName, "");

                            customPictures.Add(card);
                        }

                        if (resetToUseDefaultPictures)
                        {
                            break;
                        }
                    }

                    // Now use the custom pictures is we have all the required data.
                    if (!resetToUseDefaultPictures)
                    {
                        vm.SetupCustomMatchingCardList(customPictures);
                    }
                    else
                    {
                        vm.SetupDefaultMatchingCardList();
                    }
                }

                previousShowCustomPictures = showCustomPictures;
                previousPicturePath = picturePath;
            }
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
                vm.ResetGrid(true);
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
