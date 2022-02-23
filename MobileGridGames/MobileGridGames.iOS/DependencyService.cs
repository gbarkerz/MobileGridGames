using Foundation;
using MobileGridGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.iOS.MobileGridGamesPlatformAction))]
namespace MobileGridGames.iOS
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        public void ScreenReaderAnnouncement(string notification)
        {
            // Future: Make VoiceOver announcement.
        }

        public void SetAppTheme(App.Theme mode)
        {
            // Future: Set current theme.
        }
    }
}
