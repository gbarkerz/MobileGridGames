using System;
using System.Globalization;
using Xamarin.Forms;

namespace MobileGridGames.Views
{
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

            // Todo: Properly account for line height etc. For now, just shrink the value.
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

    public class GameIsNotReadyToSquaresOpacity : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gameIsNotReady = (bool)value;

            double squareListOpacity = (gameIsNotReady ? 0.3 : 1.0);

            return squareListOpacity;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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
