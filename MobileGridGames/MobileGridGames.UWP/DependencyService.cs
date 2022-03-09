using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MobileGridGames.Services;

using static MobileGridGames.App;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.UWP.MobileGridGamesPlatformAction))]
namespace MobileGridGames.UWP
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        public void ScreenReaderAnnouncement(string notification)
        {
            // Future: Add this action.
        }
    }
}