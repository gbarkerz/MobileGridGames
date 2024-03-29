﻿using MobileGridGames.ResX;
using Xamarin.Essentials;

namespace MobileGridGames.ViewModels
{
    // View model for the Where's WCAG Settings page in the app.
    public class WheresSettingsViewModel : BaseViewModel
    {
        public WheresSettingsViewModel()
        {
            Title = AppResources.ResourceManager.GetString("WheresSettings");
        }

        private bool playSoundOnMatch;
        public bool PlaySoundOnMatch
        {
            get
            {
                return playSoundOnMatch;
            }
            set
            {
                if (playSoundOnMatch != value)
                {
                    SetProperty(ref playSoundOnMatch, value);

                    Preferences.Set("WheresPlaySoundOnMatch", value);
                }
            }
        }

        private bool playSoundOnNotMatch;
        public bool PlaySoundOnNotMatch
        {
            get
            {
                return playSoundOnNotMatch;
            }
            set
            {
                if (playSoundOnNotMatch != value)
                {
                    SetProperty(ref playSoundOnNotMatch, value);

                    Preferences.Set("WheresPlaySoundOnNotMatch", value);
                }
            }
        }
    }
}
