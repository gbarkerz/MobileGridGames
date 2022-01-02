
using Xamarin.Essentials;

namespace MobileGridGames.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel()
        {
            Title = "Settings";
        }

        private bool showNumbers;
        public bool ShowNumbers
        {
            get => showNumbers;
            set => SetProperty(ref showNumbers, value);
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
    }
}