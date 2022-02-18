using MobileGridGames.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchingGameSettingsPage : ContentPage
    {
        // For now, there are exactly 8 different pairs of cards in the game.
        private int countCardPairs = 8;

        public MatchingGameSettingsPage()
        {
            InitializeComponent();

            this.BindingContext = new MatchingSettingsViewModel();

            var vm = this.BindingContext as MatchingSettingsViewModel;
            vm.PlaySoundOnMatch = Preferences.Get("PlaySoundOnMatch", true);
            vm.PlaySoundOnNotMatch = Preferences.Get("PlaySoundOnNotMatch", true);
            vm.ShowCustomPictures = Preferences.Get("ShowCustomPictures", false);
            vm.PicturePathMatching = Preferences.Get("PicturePathMatching", "");

            LoadCustomPictureData();
        }

        private void LoadCustomPictureData()
        {
            var vm = this.BindingContext as MatchingSettingsViewModel;

            // Check we have a filepath for custom pictures.
            if (String.IsNullOrEmpty(vm.PicturePathMatching))
            {
                return;
            }

            vm.PictureListCollection.Clear();

            bool resetCustomPictureData = false;

            // Load up the persisted details for each picture.
            for (int i = 0; i < countCardPairs; i++)
            {
                var item = new PictureData();

                item.Index = i + 1;

                string settingName = "Card" + (i + 1) + "Path";
                var fullpath = Preferences.Get(settingName, "");
                if (String.IsNullOrWhiteSpace(fullpath))
                {
                    // The filepath to a picture is missing.
                    resetCustomPictureData = true;

                    break;
                }

                item.FullPath = fullpath;
                item.FileName = Path.GetFileName(fullpath);

                settingName = "Card" + (i + 1) + "Name";
                item.AccessibleName = Preferences.Get(settingName, "");
                if (String.IsNullOrWhiteSpace(item.AccessibleName))
                {
                    // The accessible name of a picture is missing.
                    resetCustomPictureData = true;

                    break;
                }

                settingName = "Card" + (i + 1) + "Description";
                item.AccessibleDescription = Preferences.Get(settingName, "");

                vm.PictureListCollection.Add(item);
            }

            if (resetCustomPictureData)
            {
                ResetCustomPictureData();
            }
        }

        private void ResetCustomPictureData()
        {
            var vm = this.BindingContext as MatchingSettingsViewModel;

            // Clear all cached and persisted data related to the use of custom pictures.
            vm.PicturePathMatching = "";
            vm.ShowCustomPictures = false;

            vm.PictureListCollection.Clear();

            for (int i = 0; i < countCardPairs; i++)
            {
                string settingName = "Card" + (i + 1) + "Path";
                Preferences.Set(settingName, "");

                settingName = "Card" + (i + 1) + "Name";
                Preferences.Set(settingName, "");

                settingName = "Card" + (i + 1) + "Description";
                Preferences.Set(settingName, "");
            }
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            SaveCurrentSettings();

            await Navigation.PopModalAsync();
        }

        private void PictureClearButton_Clicked(object sender, EventArgs e)
        {
            ResetCustomPictureData();
        }

        private async void PictureBrowseButton_Clicked(object sender, EventArgs e)
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a picture from a folder containing only 8 pictures."
            };

            try
            {
                // Pick a picture file from a folder containing the 8 custom pictures.
                // Note that this doesn't work if selecting a picture from a OneDrive folder.

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    var pathToPictures = result.FullPath;

                    // The selected folder must contain exactly the required number of pictures in it.
                    var picturePathIsValid = await IsPicturePathValid(pathToPictures);
                    if (picturePathIsValid)
                    {
                        // Check the file containing the accessible details for the pictures exists in the folder.
                        string importFile = Path.GetDirectoryName(pathToPictures) +
                                                "/MatchingGamePictureDetails.txt";

                        if (!File.Exists(importFile))
                        {
                            await DisplayAlert(
                                "Pairs Settings",
                                "The folder containing the 8 picture must also contain a MatchingGamePictureDetails file. " +
                                    "Please review the Pairs Help for more details.",
                                "OK");

                            return;
                        }

                        StreamReader streamReader = null;
                        if ((streamReader = new StreamReader(importFile)) != null)
                        {
                            // We have 8 picture files and an accessible details file,
                            // so attempt to load up everything.
                            var vm = this.BindingContext as MatchingSettingsViewModel;
                            vm.PictureListCollection.Clear();

                            vm.PicturePathMatching = Path.GetDirectoryName(pathToPictures);

                            var folder = Path.GetDirectoryName(pathToPictures);
                            DirectoryInfo di = new DirectoryInfo(folder);

                            string[] extensions = { ".jpg", ".png", ".bmp" };

                            var files = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                                            .Where(f => extensions.Contains(f.Extension.ToLower()))
                                            .ToArray();

                            // First create the new set of 8 PictureData objects.
                            for (int i = 0; i < files.Length; i++)
                            {
                                var filepath = files[i];
                                var item = new PictureData();
                                item.Index = i + 1;
                                item.FullPath = filepath.FullName;
                                item.FileName = Path.GetFileName(filepath.FullName);
                                vm.PictureListCollection.Add(item);
                            }

                            // Now populate the PictureData with the accessible names and descriptions.
                            string content = null;
                            while ((content = streamReader.ReadLine()) != null)
                            {
                                if (!SetNameDescription(content))
                                {
                                    await DisplayAlert(
                                        "Pairs Settings",
                                        "The MatchingGamePictureDetails file does not contain the expected data. " +
                                            "Please review the Pairs Help for more details.",
                                        "OK");

                                    // The expected data was missing from the file, so reset.
                                    ResetCustomPictureData();

                                    break;
                                }
                            }

                            streamReader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MobileGridGames: Browse exception: " + ex.Message);

                ResetCustomPictureData();
            }
        }

        // For now, a folder is considered valid if it contains exactly 8 files
        // with extensions suggesting that the game can handle them.
        public async Task<bool> IsPicturePathValid(string picturePath)
        {
            bool picturePathValid = true;

            try
            {
                var folder = Path.GetDirectoryName(picturePath);
                DirectoryInfo di = new DirectoryInfo(folder);

                string[] extensions = { ".jpg", ".png", ".bmp" };

                var files = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                                .Where(f => extensions.Contains(f.Extension.ToLower()))
                                .ToArray();

                if (files.Length != 8)
                {
                    picturePathValid = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Pairs: IsPicturePathValid " + ex.Message);

                picturePathValid = false;
            }

            if (!picturePathValid)
            {
                await DisplayAlert(
                    "Pairs Settings",
                    "Please choose a picture in a folder that contains exactly 8 pictures.",
                    "OK");
            }

            return picturePathValid;
        }

        private bool SetNameDescription(string content)
        {
            var fileNameDelimiter = content.IndexOf('\t');

            // Account for the string containing no tabs at all.
            if (fileNameDelimiter == -1)
            {
                // There is no Name, so do nothing here.
                return false;
            }

            string fileName = content.Substring(0, fileNameDelimiter);

            var vm = this.BindingContext as MatchingSettingsViewModel;

            // Find the PictureData object in our current collection which has the
            // same name as that found in the accessible details file.
            for (int i = 0; i < countCardPairs; i++)
            {
                if (fileName == vm.PictureListCollection[i].FileName)
                {
                    // We have a match, so set the accessible name and description.
                    string details = content.Substring(fileNameDelimiter + 1);

                    var nameDelimiter = details.IndexOf('\t');

                    string name = "";
                    string description = "";

                    // Account for the string containing no tab following the Name.
                    if (nameDelimiter != -1)
                    {
                        name = details.Substring(0, nameDelimiter);
                        description = details.Substring(nameDelimiter + 1);
                    }
                    else
                    {
                        // Leave the description empty.
                        name = details;
                    }

                    if (String.IsNullOrWhiteSpace(name))
                    {
                        // A picture must an associated accessible name.
                        return false;
                    }

                    vm.PictureListCollection[i].AccessibleName = name;
                    vm.PictureListCollection[i].AccessibleDescription = description;

                    break;
                }
            }

            return true;
        }

        private void SaveCurrentSettings()
        {
            var vm = this.BindingContext as MatchingSettingsViewModel;

            // The Settings window is being closed, so persist whatever picture data we currently have.
            if (!String.IsNullOrWhiteSpace(vm.PicturePathMatching))
            {
                for (int i = 0; i < countCardPairs; i++)
                {
                    if (vm.PictureListCollection.Count > i)
                    {
                        string settingName = "Card" + (i + 1) + "Path";
                        Preferences.Set(settingName, vm.PictureListCollection[i].FullPath);

                        settingName = "Card" + (i + 1) + "Name";
                        Preferences.Set(settingName, vm.PictureListCollection[i].AccessibleName);

                        settingName = "Card" + (i + 1) + "Description";
                        Preferences.Set(settingName, vm.PictureListCollection[i].AccessibleDescription);
                    }
                }
            }
        }
    }
}