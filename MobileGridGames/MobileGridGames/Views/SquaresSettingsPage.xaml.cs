using MobileGridGames.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SquaresSettingsPage : ContentPage
    {
        public SquaresSettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new SquareSettingsViewModel();

            var vm = this.BindingContext as SquareSettingsViewModel;
            vm.ShowNumbers = Preferences.Get("ShowNumbers", true);
            vm.NumberSizeIndex = Preferences.Get("NumberSizeIndex", 1);
            vm.ShowPicture = Preferences.Get("ShowPicture", false);
            vm.PicturePathSquares = Preferences.Get("PicturePathSquares", "");
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private void PictureClearButton_Clicked(object sender, EventArgs e)
        {
            ResetPictureData();
        }

        private void ResetPictureData()
        {
            var vm = this.BindingContext as SquareSettingsViewModel;

            // Clear all cached and persisted data related to the use of custom pictures.
            vm.PicturePathSquares = "";
            vm.ShowPicture = false;
        }

        private async void PictureBrowseButton_Clicked(object sender, EventArgs e)
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a background picture."
            };

            try
            {
                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    var settingsViewModel = this.BindingContext as SquareSettingsViewModel;

                    settingsViewModel.PicturePathSquares = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MobileGridGames: Browse exception: " + ex.Message);
            }
        }
    }
}
