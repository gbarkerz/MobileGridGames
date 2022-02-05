using MobileGridGames.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MobileGridGames.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
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

        public void RaiseNotificationEvent(string notification)
        {
            Debug.WriteLine("MobileGridGames: Announcing \"" + notification + "\"");

            var service = DependencyService.Get<IMobileGridGamesPlatformAction>();
            service.ScreenReaderAnnouncement(notification);
        }
    }
}
