namespace MobileGridGames.Services
{
    public interface IMobileGridGamesPlatformAction
    {
        void ScreenReaderAnnouncement(string notification);

        void SetAppTheme(App.Theme theme);
    }
}
