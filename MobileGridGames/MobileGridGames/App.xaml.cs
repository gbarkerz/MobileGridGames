using MobileGridGames.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileGridGames
{
    public partial class App : Application
    {
        public enum Theme
        {
            Unspecified,
            Light,
            Dark
        }

        public App()
        {
            //Register your Syncfusion license.
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("<Insert your licence key here.>");

            InitializeComponent();

            var showDarkTheme = Preferences.Get("ShowDarkTheme", false);
            DependencyService.Get<IMobileGridGamesPlatformAction>().SetAppTheme(
                (showDarkTheme ? Theme.Dark : Theme.Light));

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
