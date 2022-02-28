using MobileGridGames.ResX;
using MobileGridGames.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames.ViewModels
{
    // View model for the Squares Settings page in the app.
    public class AppSettingsViewModel : BaseViewModel
    {
        public AppSettingsViewModel()
        {
            Title = AppResources.ResourceManager.GetString("AppSettings");
        }

        private bool showDarkTheme;
        public bool ShowDarkTheme
        {
            get
            {
                return showDarkTheme;
            }
            set
            {
                if (showDarkTheme != value)
                {
                    SetProperty(ref showDarkTheme, value);

                    Preferences.Set("ShowDarkTheme", value);

                    DependencyService.Get<IMobileGridGamesPlatformAction>().SetAppTheme(
                        showDarkTheme ? App.Theme.Dark : App.Theme.Light);
                }
            }
        }

        // Use "new" hear to make it unabiguous between the BaseViewModel HideGrid property.
        private bool hideGrid;
        public new bool HideGrid
        {
            get
            {
                return hideGrid;
            }
            set
            {
                if (hideGrid != value)
                {
                    SetProperty(ref hideGrid, value);

                    Preferences.Set("HideGrid", value);
                }
            }
        }

    }
}
