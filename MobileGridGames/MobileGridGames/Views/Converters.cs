using MobileGridGames.ViewModels;
using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames.Views
{
    // Converts the CollectionView height into the height of each row in the grid.
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

    // Converts the NumberSizeIndex into a proportion of the row height for a grid to contain the number.
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

    // Converts the height of the grid containing the number shown in the square to the number's FontSize.
    public class LabelContainerHeightToFontSize : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((values == null) || (values.Length < 2) || (values[0] == null) || (values[1] == null))
            {
                return 0;
            }

            var showNumbers = (bool)values[0];
            var containerHeightPixels = (double)values[1];

            // Future: Properly account for line height etc. For now, just shrink the value.
            // Also this reduces the size to account for tall cells in portrait orientation.
            double fontHeightPoints = 0;

            if (showNumbers)
            {
                fontHeightPoints = containerHeightPixels * 0.6;
            }

            return fontHeightPoints;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converts the square's TargetIndex to an IsVisible on the Frame containing the number shown on the square.
    public class SquareTargetIndexToContainerFrameVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var targetIndex = (int)value;

            // The Frame on the empty square is not visible.
            return (targetIndex != 15);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converts the ShowPicture setting and square's TargetIndex to the IsVisible on the square's image.
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

            // Only show a picture if pictures are to be shown and this is not the empty square.
            return picturesVisible && (targetIndex != 15);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convert the app's GameIsLoading to the opacity of the squares in the grid.
    public class GameIsLoadingToSquaresOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gameIsLoading = (bool)value;

            double squareListOpacity = (gameIsLoading ? 0.3 : 1.0);

            return squareListOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convert the path to the picture to the IsVisible on a static label associated with the picture path.
    public class SettingsPicturePathToPicturePathLabelIsVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var picturePath = (string)value;

            bool picturePathLabelIsVisible = !String.IsNullOrWhiteSpace(picturePath);

            return picturePathLabelIsVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GameIsLoadingToFlyoutBehavior : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gameIsLoadingToFlyoutBehavior = (bool)value;

            return (gameIsLoadingToFlyoutBehavior ?
                        FlyoutBehavior.Disabled : FlyoutBehavior.Flyout);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GameIsLoadingToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isGameLoading = (bool)value;

            return (isGameLoading ? false : true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CardToCollectionViewIndex : IValueConverter
    {
        private static String[] numberWords = { 
            "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", 
            "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var card = (Card)value;
            if (card == null)
            {
                return -1;
            }

            var binding = (Binding)parameter;
            var collectionView = (CollectionView)binding.Source;

            var vm = collectionView.BindingContext as MatchingViewModel;

            var collectionViewIndex = vm.SquareListCollection.IndexOf(card);

            // Return a word here, to avoid speech of "1" being ambiguous between
            // 1, 10, 11, etc.
            return numberWords[collectionViewIndex];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MatchingSettingsPicturePathToIsVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var picturePath = (string)value;

            bool picturePathLabelIsVisible = String.IsNullOrWhiteSpace(picturePath);

            return picturePathLabelIsVisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
