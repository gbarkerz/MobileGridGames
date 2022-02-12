using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

            this.PictureListCollection = new ObservableCollection<PictureData>();
        }

        private ObservableCollection<PictureData> pictureList;
        public ObservableCollection<PictureData> PictureListCollection
        {
            get { return pictureList; }
            set { this.pictureList = value; }
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

        private bool showCustomPictures = false;
        public bool ShowCustomPictures
        {
            get => Preferences.Get("ShowCustomPictures", false);
            set
            {
                if (showCustomPictures != value)
                {
                    Preferences.Set("ShowCustomPictures", value);
                    SetProperty(ref showCustomPictures, value);
                }
            }
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

    public class PictureData : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public string FileName { get; set; }

        private string fullPath;
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                SetProperty(ref fullPath, value);
            }
        }

        private string accessibleName;
        public string AccessibleName
        {
            get { return accessibleName; }
            set
            {
                SetProperty(ref accessibleName, value);
            }
        }

        private string accessibleDescription;
        public string AccessibleDescription
        {
            get { return accessibleDescription; }
            set
            {
                SetProperty(ref accessibleDescription, value);
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
