using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MobileGridGames.ViewModels;
using Xamarin.Forms;

namespace MobileGridGames.Services
{
    public interface IMobileGridGamesPlatformAction
    {
        void ScreenReaderAnnouncement(string notification);
        Task<string> GetPairsPictureFolder();
    }
}
