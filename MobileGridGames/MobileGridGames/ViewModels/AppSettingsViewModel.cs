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
            Title = "App Settings";
        }

        private bool showDarkTheme;
        public bool ShowDarkTheme
        {
            get => Preferences.Get("ShowDarkTheme", false);
            set
            {
                if (showDarkTheme != value)
                {
                    Preferences.Set("ShowDarkTheme", value);
                    SetProperty(ref showDarkTheme, value);

                    DependencyService.Get<IMobileGridGamesPlatformAction>().SetAppTheme(
                        showDarkTheme ? App.Theme.Dark : App.Theme.Light);
                }
            }
        }
    }
}
