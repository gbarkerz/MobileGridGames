using Xamarin.Essentials;

namespace MobileGridGames.ViewModels
{
    // View model for the Squares Settings page in the app.
    public class SquareSettingsViewModel : BaseViewModel
    {
        public SquareSettingsViewModel()
        {
            Title = "Squares Settings";
        }

        private bool showNumbers;
        public bool ShowNumbers
        {
            get
            {
                return showNumbers;
            }
            set
            {
                if (showNumbers != value)
                {
                    Preferences.Set("ShowNumbers", value);
                    SetProperty(ref showNumbers, value);
                }
            }
        }

        private int numberSizeIndex;
        public int NumberSizeIndex
        {
            get
            {
                return numberSizeIndex;
            }
            set
            {
                if (numberSizeIndex != value)
                {
                    Preferences.Set("NumberSizeIndex", value);
                    SetProperty(ref numberSizeIndex, value);
                }
            }
        }

        private bool showPicture;
        public bool ShowPicture
        {
            get
            {
                return showPicture;
            }
            set
            {
                if (showPicture != value)
                {
                    Preferences.Set("ShowPicture", value);
                    SetProperty(ref showPicture, value);
                }
            }
        }

        private string picturePathSquares;
        public string PicturePathSquares
        {
            get
            {
                return picturePathSquares;
            }
            set
            {
                if (picturePathSquares != value)
                {
                    Preferences.Set("PicturePathSquares", value);
                    SetProperty(ref picturePathSquares, value);
                }
            }
        }
    }
}
