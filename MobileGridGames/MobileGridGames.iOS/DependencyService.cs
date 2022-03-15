using Foundation;
using MobileGridGames.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

using MobileGridGames.iOS;
using static MobileGridGames.App;
using MobileGridGames.Styles;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.iOS.MobileGridGamesPlatformAction))]
namespace MobileGridGames.iOS
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        public void ScreenReaderAnnouncement(string notification)
        {
            // Take the action described at:
            // https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/accessibility

            UIAccessibility.PostNotification(
              UIAccessibilityPostNotification.Announcement,
                new NSString(notification));
        }
    }
}
