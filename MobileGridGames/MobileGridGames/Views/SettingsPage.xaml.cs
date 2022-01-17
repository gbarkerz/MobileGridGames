using MobileGridGames.ViewModels;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new SettingsViewModel();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
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
                    var settingsViewModel = this.BindingContext as SettingsViewModel;

                    settingsViewModel.PicturePath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MobileGridGames: Browse exception: " + ex.Message);
            }
        }
    }
}
