using System.Diagnostics;
using MobileGridGames.Services;
using Windows.UI.Xaml.Automation.Peers;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.UWP.MobileGridGamesPlatformAction))]
namespace MobileGridGames.UWP
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        private AutomationPeer screenReaderPeer;
        private string screenReaderAnnouncementGuid = "F614FC96-70E0-40EB-ACF6-4A5F30E38A45";

        public void SetScreenReaderAnnouncementPeer(AutomationPeer screenReaderPeer)
        {
            this.screenReaderPeer = screenReaderPeer;
        }

        public void ScreenReaderAnnouncement(string notification)
        {
            if (screenReaderPeer != null)
            {
                Debug.WriteLine("UWP Grid Games - Announce: \"" + notification + "\"");

                // Raise a UIA Notification event.
                screenReaderPeer.RaiseNotificationEvent(
                     AutomationNotificationKind.Other,
                     AutomationNotificationProcessing.All,
                     notification,
                     screenReaderAnnouncementGuid);
            }
        }
    }
}