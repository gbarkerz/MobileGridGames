//using MobileGridGames.Services;
using MobileGridGames.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileGridGames
{
    public partial class App : Application
    {

        public App()
        {
            //Register your Syncfusion license.
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("<Insert your licence key here.>");

            InitializeComponent();

            // Todo: Register DependencyService here.
            //DependencyService.Register<MockDataStore>();
            
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

    public interface IMobileGridGamesPlatformAction
    {
        void GetSquareBitmap(object image);
    }
}
