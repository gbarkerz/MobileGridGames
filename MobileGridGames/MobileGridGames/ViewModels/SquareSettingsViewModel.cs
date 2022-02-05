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
            get => Preferences.Get("ShowNumbers", true);
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
            get => Preferences.Get("NumberSizeIndex", 1);
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
            get => Preferences.Get("ShowPicture", false);
            set
            {
                if (showPicture != value)
                {
                    Preferences.Set("ShowPicture", value);
                    SetProperty(ref showPicture, value);
                }
            }
        }

        private string picturePath;
        public string PicturePath
        {
            get => Preferences.Get("PicturePath", "");
            set
            {
                if (picturePath != value)
                {
                    Preferences.Set("PicturePath", value);
                    SetProperty(ref picturePath, value);
                }
            }
        }
    }
}
