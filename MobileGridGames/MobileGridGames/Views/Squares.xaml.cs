using MobileGridGames.ViewModels;
using System;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames.Views
{
    public partial class SquaresPage : ContentPage
    {
        public SquaresPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Todo: Bind the UI directly to the Settings view model. Until then, set the
            // number size in the Square view model whenever the Sqaures page is shown.
            var vm = this.BindingContext as SquaresViewModel;
            vm.ShowNumbers = Preferences.Get("ShowNumbers", true);
            vm.NumberHeight = Preferences.Get("NumberSizeIndex", 1);
        }

        // TODO: Remove this code-behind, and bind the SelectionChanged event directly to action in the view model.
        private async void SquaresGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                var vm = this.BindingContext as SquaresViewModel;
                bool gameIsWon = await vm.AttemptMove(e.CurrentSelection[0]);
                if (gameIsWon)
                {
                    // Todo: Localize this.
                    var answer = await DisplayAlert(
                        "Congratulations!",
                        "You won the game.\r\n\r\nWould you like to play another game?",
                        "Yes", "No");
                    if (answer)
                    {
                        vm.ResetGrid();
                    }
                }
            }
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

            double gridRowHeight = 0.5;

            switch (numberSizeIndex)
            {
                case 0:
                    gridRowHeight = 0.25;
                    break;
                case 2:
                    gridRowHeight = 0.75;
                    break;
                case 3:
                    gridRowHeight = 1.0;
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
            double fontHeightPoints = containerHeightPixels * 0.5;

            return fontHeightPoints;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


