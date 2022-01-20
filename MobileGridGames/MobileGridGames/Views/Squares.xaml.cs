using MobileGridGames.ViewModels;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

// Notes:
// - I tried to remove all specifying of colour completely, to rely only on default colors.
//   but the Shell bar still showed blue. (Setting the Shell BackgroundColor transparent
//   didn't affect that.) So I restored all the app template use of blue and white.
// - The app does not set any explicity font height, but does set some proportional text size.
// - Everything needs to be localised.

namespace MobileGridGames.Views
{
    public partial class SquaresPage : ContentPage
    {
        private string previousLoadedPicture = "";

        public SquaresPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Todo: Bind the UI directly to the Settings view model. Until then, set the
            // number size in the Square view model whenever the Squares page is shown.
            var vm = this.BindingContext as SquaresViewModel;
            vm.ShowNumbers = Preferences.Get("ShowNumbers", true);
            vm.NumberHeight = Preferences.Get("NumberSizeIndex", 1);
            vm.ShowPicture = Preferences.Get("ShowPicture", true);
            vm.PicturePath = Preferences.Get("PicturePath", "");

            if (vm.ShowPicture && (vm.PicturePath != previousLoadedPicture))
            {
                // Prevent input on the grid while the image is being loaded into the squares.
                vm.GameIsNotReady = true;

                // Cache the path to the loaded picture.
                previousLoadedPicture = vm.PicturePath;

                vm.RestoreEmptyGrid();

                // The loading of the images into the squares is made synchronously through the 15 squares.
                nextSquareIndexForImageSourceSetting = 0;

                vm.RaiseNotificationEvent(PleaseWaitLabel.Text);

                // If the image fails to load, it seems that the ImageEditor does not throw an exception.
                // Rather, the various event handlers set up just don't get called. As such, we never 
                // know there's a problem loading the image. For now, this means that the player must
                // eventually given up waiting for the image to be loaded, and go to the app Settings 
                // to select another image.

                // Note that if an invalid filename is supplied here, the return value from ImageSource.FromFile()
                // does not suggest there's a problem, nor is a helpful event or exception thrown. So check whether
                // the file exists ourselves first.

                var fileExists = File.Exists(vm.PicturePath);
                if (fileExists)
                {
                    // Future: Verify that if the various event handlers are still being called from the
                    // previous attempt to load a picture, those event handlers will no longer be called
                    // once the loading of another picture begins.
                    GridGameImageEditor.Source = ImageSource.FromFile(vm.PicturePath);

                    Debug.WriteLine("Grid Games: ImageEditor source now " + GridGameImageEditor.Source.ToString());
                }
                else
                {
                    Debug.WriteLine("Grid Games: File not found. " + vm.PicturePath);

                    // We'll not attempt to load this picture again.
                    Preferences.Set("PicturePath", "");
                    vm.PicturePath = "";

                    vm.GameIsNotReady = false;
                }
            }
            else
            {
                vm.GameIsNotReady = false;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var itemGrid = (Grid)sender;
            var itemName = AutomationProperties.GetName(itemGrid);

            Debug.WriteLine("Grid Games: Tapped on Square " + itemName);

            AttemptToMoveSquareByName(itemName);
        }

        private void AttemptToMoveSquareByName(string itemName)
        {
            var vm = this.BindingContext as SquaresViewModel;

            int itemIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (vm.SquareListCollection[i].AccessibleName == itemName)
                {
                    itemIndex = i;
                    break;
                }
            }

            if (itemIndex != -1)
            {
                vm.AttemptToMoveSquare(itemIndex);
            }
        }

        // SelectionChanged handling only exists today in the app to support Android Switch Access. 
        // SelectionChanged will also be a part of keyboard support, but currently the rest of the 
        // app is not keyboard accessible, and focus feedback is unusable on the items.

        private async void SquaresGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Grid Games: Selection changed. Selection count is " + e.CurrentSelection.Count);

            // Do nothing here if pictures have not been loaded yet onto the squares.
            var vm = this.BindingContext as SquaresViewModel;
            if (vm.GameIsNotReady)
            {
                return;
            }

            // No action required here if there is no selected item.
            if (e.CurrentSelection.Count > 0)
            {
                bool gameIsWon = vm.AttemptMoveBySelection(e.CurrentSelection[0]);

                // Clear the selection now to support the same square moving again.
                SquaresCollectionView.SelectedItem = null;

                if (gameIsWon)
                {
                    var answer = await DisplayAlert(
                        "Congratulations!",
                        "You won the game in " + vm.MoveCount + " moves.\r\n\r\nWould you like to play another game?",
                        "Yes", "No");
                    if (answer)
                    {
                        vm.ResetGrid();
                    }
                }
            }
        }

        // The remainder of this file relates to setting of pictures on the squares in the grid.

        // The use of nextSquareIndexForImageSourceSetting as the index of the square whose
        // PictureImageSource property is being set, assumes that the squares have not yet
        // been shuffled in the view model's collection of Squares.
        private int nextSquareIndexForImageSourceSetting = 0;

        private void PerformCrop()
        {
            int x = 25 * (nextSquareIndexForImageSourceSetting % 4);
            int y = 25 * (nextSquareIndexForImageSourceSetting / 4);

            Debug.WriteLine("MobileGridGames: In PerformCrop, crop at " + x + ", " + y + ".");

            GridGameImageEditor.ToggleCropping(new Rectangle(x, y, 25, 25));

            Debug.WriteLine("MobileGridGames: Called ToggleCropping.");

            // Todo: Understand why this seems to need to run on the UI thread.
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("MobileGridGames: PerformCrop, about to call Crop.");

                GridGameImageEditor.Crop();

                Debug.WriteLine("MobileGridGames: PerformCrop, called Crop.");
            });

            Debug.WriteLine("MobileGridGames: Leave PerformCrop.");
        }

        private void GridGameImageEditor_ImageLoaded(object sender, ImageLoadedEventArgs args)
        {
            Debug.WriteLine("MobileGridGames: In ImageLoaded, calling PerformCrop.");

            // Perform crop for first square.
            PerformCrop();

            Debug.WriteLine("MobileGridGames: Leave ImageLoaded, done call to PerformCrop.");
        }

        private void GridGameImageEditor_ImageEdited(object sender, ImageEditedEventArgs e)
        {
            Debug.WriteLine("MobileGridGames: In ImageEdited.");

            // If we're here following a resetting of the image, take no follow-up action.
            if (e.IsImageEdited)
            {
                // We must be here following a crop operation.
                GridGameImageEditor.Save();
            }

            Debug.WriteLine("MobileGridGames: Leave ImageEdited.");
        }

        private void GridGameImageEditor_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            Debug.WriteLine("MobileGridGames: In ImageSaving.");

            // Prevent the image from being saved to a file.
            args.Cancel = true; 

            var vm = this.BindingContext as SquaresViewModel;
            var source = GetImageSourceFromPictureStream(args.Stream);

            // We assume here that the use of nextSquareIndexForImageSourceSetting is synchronous
            // from 0 to 14.
            var square = vm.SquareListCollection[nextSquareIndexForImageSourceSetting];
            square.PictureImageSource = source;

            // Now reset the image to its original form, in order to perform the next crop.
            // Todo: Understand why this seems to need to run on the UI thread.
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("MobileGridGames: About to call Reset.");

                GridGameImageEditor.Reset();

                Debug.WriteLine("MobileGridGames: Back from call to Reset.");
            });

            Debug.WriteLine("MobileGridGames: Leave ImageSaving.");
        }

        private ImageSource GetImageSourceFromPictureStream(Stream stream)
        {
            stream.Position = 0;

            // The input stream will get closed, so create a new stream from it here.
            var buffer = new byte[stream.Length];

            MemoryStream ms = new MemoryStream();

            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            var imageSource = ImageSource.FromStream(() => new MemoryStream(buffer));

            return imageSource;
        }

        private void GridGameImageEditor_EndReset(object sender, EndResetEventArgs args)
        {
            var vm = this.BindingContext as SquaresViewModel;

            // Provide a countdown for players using sceen readers.
            if (nextSquareIndexForImageSourceSetting % 3 == 0)
            {
                var countdown = (15 - nextSquareIndexForImageSourceSetting) / 3;
                vm.RaiseNotificationEvent(countdown.ToString());
            }

            Debug.WriteLine("MobileGridGames: In EndReset.");

            // We've completed the image settings for a square. Continue with the next square if there is one.
            ++nextSquareIndexForImageSourceSetting;

            Debug.WriteLine("MobileGridGames: nextSquareIndexForImageSourceSetting now " + nextSquareIndexForImageSourceSetting);

            // If we're not done loading pictures into squares, load a picture into the next square.
            if (nextSquareIndexForImageSourceSetting < 15)
            {
                PerformCrop();
            }
            else
            {
                // We've loaded all the pictures, so shuffle them and enable the game.
                vm.GameIsNotReady = false;

                vm.ResetGrid();
            }

            Debug.WriteLine("MobileGridGames: Leave EndReset");
        }
    }
}
