using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace MobileGridGames.ViewModels
{
    // View model for the Squares Settings page in the app.
    public class MatchingSettingsViewModel : BaseViewModel
    {
        public MatchingSettingsViewModel()
        {
            Title = "Pairs Settings";
        }

        private bool playSoundOnMatch = false;
        public bool PlaySoundOnMatch
        {
            get => Preferences.Get("PlaySoundOnMatch", true);
            set
            {
                if (playSoundOnMatch != value)
                {
                    Preferences.Set("PlaySoundOnMatch", value);
                    SetProperty(ref playSoundOnMatch, value);
                }
            }
        }

        private bool playSoundOnNotMatch = false;
        public bool PlaySoundOnNotMatch
        {
            get => Preferences.Get("PlaySoundOnNotMatch", true);
            set
            {
                if (playSoundOnNotMatch != value)
                {
                    Preferences.Set("PlaySoundOnNotMatch", value);
                    SetProperty(ref playSoundOnNotMatch, value);
                }
            }
        }
    }
}
