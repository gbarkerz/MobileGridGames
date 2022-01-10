using MobileGridGames.ViewModels;
using Syncfusion.SfImageEditor.XForms;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;

// Todo: Sort out keyboard focus on the squares. Focus is almost invisible when run in the emulator.

namespace MobileGridGames.Views
{
    public partial class SquaresPage : ContentPage
    {
        private bool gridReadyForInput = false;
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

            // Dependency test.
            //var service = DependencyService.Get<IMobileGridGamesPlatformAction>();
            //var image = new Image();
            //image.Source = vm.PicturePath;
            //service.GetSquareBitmap(image);

            if (vm.ShowPicture && (vm.PicturePath != previousLoadedPicture))
            {
                gridReadyForInput = false;

                previousLoadedPicture = vm.PicturePath;

                vm.RestoreEmptyGrid();

                nextSquareIndexForImageSourceSetting = 0;

                GridGameImageEditor.Source = ImageSource.FromFile(vm.PicturePath);
            }
            else
            {
                gridReadyForInput = true;
            }
        }

        // The use of nextSquareIndexForImageSourceSetting as the index of the square whose
        // PictureImageSource property is being set, assumes that the squares have not yet
        // been shuffled in the view model's collection of Squares.
        private int nextSquareIndexForImageSourceSetting = 0;

        private void PerformCrop()
        {
            Debug.WriteLine("GB: In PerformCrop");

            int x = 25 * (nextSquareIndexForImageSourceSetting % 4);
            int y = 25 * (nextSquareIndexForImageSourceSetting / 4);

            Debug.WriteLine("GB: Crop at " + x + ", " + y);

            Debug.WriteLine("GB: About to call ToggleCropping");

            GridGameImageEditor.ToggleCropping(new Rectangle(x, y, 25, 25));

            Debug.WriteLine("GB: Called ToggleCropping");

            // Barker: Understand why this seems to need to run on the UI thread.
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("GB: About to call Crop");

                GridGameImageEditor.Crop();

                Debug.WriteLine("GB: Called Crop");
            });

            Debug.WriteLine("GB: Leave PerformCrop");
        }

        private void GridGameImageEditor_ImageLoaded(object sender, ImageLoadedEventArgs args)
        {
            Debug.WriteLine("GB: In ImageLoaded");

            // Perform crop for first square.
            PerformCrop();

            Debug.WriteLine("GB: Leave ImageLoaded");
        }

        private void GridGameImageEditor_ImageEdited(object sender, ImageEditedEventArgs e)
        {
            Debug.WriteLine("GB: In ImageEdited");

            // If we're here following a resetting of the image, we dont want to save it.
            if (e.IsImageEdited)
            {
                // We must be here following a crop operation.
                GridGameImageEditor.Save();
            }

            Debug.WriteLine("GB: Leave ImageEdited");
        }

        private void GridGameImageEditor_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            Debug.WriteLine("GB: In ImageSaving");

            // Prevent the image from being saved to a file.
            args.Cancel = true; 

            // Save the dropped date to an image.
            var vm = this.BindingContext as SquaresViewModel;
            var source = GetImageSourceFromPictureStream(args.Stream);
            var square = vm.SquareListCollection[nextSquareIndexForImageSourceSetting];
            square.PictureImageSource = source;

            // Now reset the image to its original form, in order to perform the next crop.
            // Barker: Understand why this seems to need to run on the UI thread.
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("GB: About to call Reset");

                GridGameImageEditor.Reset();

                Debug.WriteLine("GB: Back from call to Reset");
            });

            Debug.WriteLine("GB: Leave ImageSaving");
        }

        private void GridGameImageEditor_EndReset(object sender, EndResetEventArgs args)
        {
            Debug.WriteLine("GB: In EndReset");

            // We've completed the image settings for a square. Continue with the next square if there is one.
            ++nextSquareIndexForImageSourceSetting;

            Debug.WriteLine("GB: nextSquareIndexForImageSourceSetting now " + nextSquareIndexForImageSourceSetting);

            if (nextSquareIndexForImageSourceSetting < 15)
            {
                PerformCrop();
            }
            else
            {
                var vm = this.BindingContext as SquaresViewModel;
                vm.ResetGrid();
            }

            gridReadyForInput = true;

            Debug.WriteLine("GB: Leave EndReset");
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

        // TODO: Remove this code-behind, and bind the SelectionChanged event directly to action in the view model.
        private async void SquaresGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Do nothing here if pictures have not been loaded yet onto the squares.
            if (!gridReadyForInput)
            {
                return;
            }

            // No action required here if there is no selected item.
            if (e.CurrentSelection.Count > 0)
            {
                var vm = this.BindingContext as SquaresViewModel;
                bool gameIsWon = await vm.AttemptMove(e.CurrentSelection[0]);

                // Clear the selection now to support the same square moving again.
                SquaresCollectionView.SelectedItem = null;

                if (gameIsWon)
                {
                    // Todo: Localize this.
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            SquaresCollectionView.IsVisible = true;
        }
    }

    public class CollectionViewHeightToRowHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value / 4) - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberSizeIndexToGridRowHeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numberSizeIndex = (int)value;

            double gridRowHeight = 0.35;

            switch (numberSizeIndex)
            {
                case 0:
                    gridRowHeight = 0.2;
                    break;
                case 2:
                    gridRowHeight = 0.5;
                    break;
                case 3:
                    gridRowHeight = 0.65;
                    break;
                default:
                    break;
            }

            if ((string)parameter == "1")
            {
                gridRowHeight = 1.0 - gridRowHeight;
            }

            return new GridLength(gridRowHeight, GridUnitType.Star);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LabelContainerHeightToFontSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var containerHeightPixels = (double)value;

            // Todo: Properly account for line height etc. For now, just shrink the value.
            // Also this reduces the size to account for tall cells in portrait orientation.
            double fontHeightPoints = containerHeightPixels * 0.8;

            return fontHeightPoints;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SquareTargetIndexToContainerFrameVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var targetIndex = (int)value;

            return (targetIndex != 15);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SquareTargetIndexToIsVisible : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values == null) || (values.Length < 2) || (values[0] == null) || (values[1] == null))
            {
                return 0;
            }

            var targetIndex = (int)values[0];
            var picturesVisible = (bool)values[1];

            return picturesVisible && (targetIndex != 15);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SquareTargetIndexToImageTranslationX : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values == null) || (values.Length < 2) || (values[0] == null) || (values[1] == null))
            {
                return 0;
            }

            var targetIndex = (int)values[0];
            var columnIndex = targetIndex % 4;

            var collectionViewWidth = (double)values[1];
            var columnWidth = collectionViewWidth / 4;

            double multiplier = Utils.GetMultiplierFromRowColumnIndex(columnIndex);

            double imageOffset = multiplier * columnWidth;

            return imageOffset;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SquareTargetIndexToImageTranslationY : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values == null) || (values.Length < 2) || (values[0] == null) || (values[1] == null))
            {
                return 0;
            }

            var targetIndex = (int)values[0];
            var rowIndex = targetIndex / 4;

            var collectionViewHeight = (double)values[1];
            var columnHeight = collectionViewHeight / 4;

            double multiplier = Utils.GetMultiplierFromRowColumnIndex(rowIndex);

            double imageOffset = multiplier * columnHeight;

            return imageOffset;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Utils
    {
        static public double GetMultiplierFromRowColumnIndex(int index)
        {
            double multiplier = 0;
            switch (index)
            {
                case 0:
                    multiplier = 1.5;
                    break;
                case 1:
                    multiplier = 0.5;
                    break;
                case 2:
                    multiplier = -0.5;
                    break;
                default:
                    multiplier = -1.5;
                    break;
            }

            return multiplier;
        }
    }
}


