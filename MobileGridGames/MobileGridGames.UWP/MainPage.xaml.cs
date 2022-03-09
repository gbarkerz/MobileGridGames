using MobileGridGames.Services;
using Windows.UI.Xaml.Automation.Peers;
using Xamarin.Forms;

namespace MobileGridGames.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new MobileGridGames.App());

            // In order to raise a custom screen reader notification later,
            // we need to supply a XAML FrameworkElement's AutomationPeer.
            var peer = FrameworkElementAutomationPeer.FromElement(
                TextBlockForScreenReaderAnnouncements);
            if (peer != null)
            {
                // Future: This is called after the first attempt to raise a screen reader
                // annoucement has been made in the game, and so that announcement was lost.
                // So figure out what to do about that.

                var service = DependencyService.Get<IMobileGridGamesPlatformAction>();
                (service as MobileGridGamesPlatformAction).SetScreenReaderAnnouncementPeer(peer);
            }
        }
    }
}
