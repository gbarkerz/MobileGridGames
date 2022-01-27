using MobileGridGames.ViewModels;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

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

            // Account for the app settings changing since the page was last shown.
            var vm = this.BindingContext as SquaresViewModel;
            vm.ShowNumbers = Preferences.Get("ShowNumbers", true);
            vm.NumberHeight = Preferences.Get("NumberSizeIndex", 1);
            vm.ShowPicture = Preferences.Get("ShowPicture", false);
            vm.PicturePath = Preferences.Get("PicturePath", "");

            // Has the state of the picture being shown changed since we were last changed?
            if (vm.ShowPicture && (vm.PicturePath != previousLoadedPicture))
            {
                // Prevent input on the grid while the image is being loaded into the squares.
                vm.GameIsNotReady = true;

                // Restore the order of the squares in the grid.
                vm.RestoreEmptyGrid();

                // The loading of the images into the squares is made synchronously through the first 15 squares.
                nextSquareIndexForImageSourceSetting = 0;

                // If an invalid filename is supplied here, (for example, a previously valid image has been
                // deleted from the device), it seems that the return value from ImageSource.FromFile() does
                // not suggest there's a problem, nor is a helpful event or exception thrown. So check whether
                // the file exists ourselves first.

                // Future: IMPORTANT! Handle an attempt to load a file which isn't handled by the ImageEditor.
                // As things are today, if a file isn't loaded the app is left in a state where the game's not
                // ready to play, and a valid file can't then be selected and loaded. 

                if (vm.IsImageFilePathValid(vm.PicturePath))
                {
                    vm.RaiseNotificationEvent(PleaseWaitLabel.Text);

                    // Future: Verify that if the various event handlers are still being called from the
                    // previous attempt to load a picture, those event handlers will no longer be called
                    // once the loading of another picture begins.
                    GridGameImageEditor.Source = ImageSource.FromFile(vm.PicturePath);

                    Debug.WriteLine("Grid Games: ImageEditor source now " + GridGameImageEditor.Source.ToString());
                }
                else
                {
                    Debug.WriteLine("Grid Games: Valid image file not found. " + vm.PicturePath);

                    // We'll not attempt to load this picture again.
                    Preferences.Set("PicturePath", "");
                    vm.PicturePath = "";

                    vm.GameIsNotReady = false;
                }

                // Cache the path to the loaded picture.
                previousLoadedPicture = vm.PicturePath;
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var itemGrid = (Grid)sender;
            var itemAccessibleName = AutomationProperties.GetName(itemGrid);

            Debug.WriteLine("Grid Games: Tapped on Square " + itemAccessibleName);

            AttemptToMoveSquareByName(itemAccessibleName);
        }

        private async void AttemptToMoveSquareByName(string itemName)
        {
            var vm = this.BindingContext as SquaresViewModel;

            int itemIndex = GetItemCollectionIndexFromItemAccessibleName(itemName);
            if (itemIndex != -1)
            {
                bool gameIsWon = vm.AttemptToMoveSquare(itemIndex);
                if (gameIsWon)
                {
                    await OfferToRestartGame();
                }
            }
        }

        private int GetItemCollectionIndexFromItemAccessibleName(string ItemAccessibleName)
        {
            var vm = this.BindingContext as SquaresViewModel;

            int itemIndex = -1;
            for (int i = 0; i < 16; ++i)
            {
                if (vm.SquareListCollection[i].AccessibleName == ItemAccessibleName)
                {
                    itemIndex = i;
                    break;
                }
            }

            return itemIndex;
        }

        private async Task OfferToRestartGame()
        {
            var vm = this.BindingContext as SquaresViewModel;

            var answer = await DisplayAlert(
                "Congratulations!",
                "You won the game in " + vm.MoveCount + " moves.\r\n\r\nWould you like to play another game?",
                "Yes", "No");
            if (answer)
            {
                vm.ResetGrid();
            }
        }

        // SelectionChanged handling only exists today in the app to support Android Switch Access. 
        // At some point SelectionChanged may also be a part of keyboard support, but currently the
        // rest of the app is not keyboard accessible, and focus feedback is unusable on the items.

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
                    await OfferToRestartGame();
                }
            }
        }

        // Important: The remainder of this file relates to setting of pictures on the squares in the
        // grid. However, the threading model for the various event handlers below is not understood,
        // so while the code seems to work it seems almost certain that the code will change once the
        // threading model is understood.

        // The use of nextSquareIndexForImageSourceSetting as the index of the square whose
        // PictureImageSource property is being set, assumes that the squares have not yet
        // been shuffled in the view model's collection of Squares.
        private int nextSquareIndexForImageSourceSetting = 0;

        // ImageLoad is called once following the picture being set on the control.
        private void GridGameImageEditor_ImageLoaded(object sender, ImageLoadedEventArgs args)
        {
            Debug.WriteLine("MobileGridGames: In ImageLoaded, calling PerformCrop.");

            if (nextSquareIndexForImageSourceSetting != 0)
            {
                Debug.WriteLine("MobileGridGames: Error in ImageLoaded, nextSquareIndexForImageSourceSetting should be zero, " +
                    nextSquareIndexForImageSourceSetting.ToString());

                return;
            }

            // Perform a crop for first square.
            PerformCrop();

            Debug.WriteLine("MobileGridGames: Leave ImageLoaded, done call to PerformCrop.");
        }

        private void PerformCrop()
        {
            // The x,y values for cropping are a percentage of the full image size.
            int x = 25 * (nextSquareIndexForImageSourceSetting % 4);
            int y = 25 * (nextSquareIndexForImageSourceSetting / 4);

            Debug.WriteLine("MobileGridGames: In PerformCrop, crop at " + x + ", " + y + ".");

            // Set up the bounds for the next crop operation.
            GridGameImageEditor.ToggleCropping(new Rectangle(x, y, 25, 25));

            Debug.WriteLine("MobileGridGames: Called ToggleCropping.");

            // Future: Understand why Crop() seems to need to run on the UI thread.
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("MobileGridGames: PerformCrop, about to call Crop.");

                GridGameImageEditor.Crop();

                Debug.WriteLine("MobileGridGames: PerformCrop, called Crop.");
            });

            Debug.WriteLine("MobileGridGames: Leave PerformCrop.");
        }

        // ImageEdited is called following a Crop operation and when the image is reset.
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

        // ImageSaving is called following each crop of the picture.
        private void GridGameImageEditor_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            Debug.WriteLine("MobileGridGames: In ImageSaving.");

            // Important: Prevent the cropped image from being saved to a file.
            args.Cancel = true; 

            var vm = this.BindingContext as SquaresViewModel;

            // Get the image data for the previous crop operation.
            var source = GetImageSourceFromPictureStream(args.Stream);

            // We assume here that the use of nextSquareIndexForImageSourceSetting is synchronous
            // as all the cropping operations are performed.

            if (nextSquareIndexForImageSourceSetting > 14)
            {
                Debug.WriteLine("MobileGridGames: Error in ImageSaving, nextSquareIndexForImageSourceSetting too high, " +
                    nextSquareIndexForImageSourceSetting);

                return;
            }

            // Set the cropped image on the next square.
            var square = vm.SquareListCollection[nextSquareIndexForImageSourceSetting];
            square.PictureImageSource = source;

            // Now reset the image to its original form, in order to perform the next crop.
            // Future: Understand why this seems to need to run on the UI thread.
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

        // EndResetis called as the original image is reset to perform the crop operation for the next square.
        private void GridGameImageEditor_EndReset(object sender, EndResetEventArgs args)
        {
            Debug.WriteLine("MobileGridGames: In EndReset.");

            var vm = this.BindingContext as SquaresViewModel;

            // Provide a countdown for players using screen readers.
            if (nextSquareIndexForImageSourceSetting % 3 == 0)
            {
                var countdown = (15 - nextSquareIndexForImageSourceSetting) / 3;
                vm.RaiseNotificationEvent(countdown.ToString());
            }

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
