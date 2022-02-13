using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views.Accessibility;
using MobileGridGames.Services;
using Java.Lang;

using MobileGridGames.Droid;
using static MobileGridGames.App;
using MobileGridGames.Styles;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.Droid.MobileGridGamesPlatformAction))]
namespace MobileGridGames.Droid
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        public void ScreenReaderAnnouncement(string notification)
        {
            if ((MainActivity.accessibilityManager != null) &&
                MainActivity.accessibilityManager.IsEnabled)
            {
                ICharSequence charSeqNotification = new Java.Lang.String(notification);

                AccessibilityEvent e = AccessibilityEvent.Obtain();

                e.EventType = (EventTypes)0x00004000; // This is the Android value.

                e.Text.Add(charSeqNotification);

                // NOTE: Announcements don't seem to interfer with other 
                // default announcements. Eg when game restarted, announcement on 
                // where focus is afterwards still gets announced. And note that
                // focus remains on card being moved from a delat card pile, and
                // so no announcement there gets announced by default anyway.

                MainActivity.accessibilityManager.SendAccessibilityEvent(e);
            }
        }

        public void SetAppTheme(App.Theme mode)
        {
            if (mode == Theme.Dark)
            {
                App.Current.Resources = new DarkTheme();
            }
            else
            {
                App.Current.Resources = new LightTheme();
            }
        }
    }
}