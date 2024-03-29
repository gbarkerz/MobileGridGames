﻿using MobileGridGames.ResX;
using MobileGridGames.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
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

            // Adding localized strings to a Picker in XAML seems complicated, so do it in code.
            SquaresNumberSizePicker.Items.Add(AppResources.ResourceManager.GetString("Small"));
            SquaresNumberSizePicker.Items.Add(AppResources.ResourceManager.GetString("Medium"));
            SquaresNumberSizePicker.Items.Add(AppResources.ResourceManager.GetString("Large"));
            SquaresNumberSizePicker.Items.Add(AppResources.ResourceManager.GetString("Largest"));

            this.BindingContext = new SquareSettingsViewModel();

            var vm = this.BindingContext as SquareSettingsViewModel;
            vm.ShowNumbers = Preferences.Get("ShowNumbers", true);
            vm.NumberSizeIndex = Preferences.Get("NumberSizeIndex", 1);
            vm.ShowPicture = Preferences.Get("ShowPicture", false);
            vm.PicturePathSquares = Preferences.Get("PicturePathSquares", "");
            vm.PictureName = Preferences.Get("PictureName", "");
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
            vm.PictureName = "";
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

                    // Copy the selected picture into a tmp folder where we can access it later.
                    var targetFolder = Path.Combine(Path.GetTempPath(), "SquaresGameCurrentPictures");
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    // Empty this dedicated tmp folder now.
                    DirectoryInfo di = new DirectoryInfo(targetFolder);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }

                    File.Copy(result.FullPath, targetFolder);

                    var filename = Path.GetFileName(result.FullPath);

                    settingsViewModel.PicturePathSquares = Path.Combine(targetFolder, filename);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MobileGridGames: Browse exception: " + ex.Message);
            }
        }
    }
}
