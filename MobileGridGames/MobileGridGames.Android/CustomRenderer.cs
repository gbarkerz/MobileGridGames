using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MobileGridGames.AppShell), typeof(MobileGridGames.Droid.CustomShellRenderer))]
namespace MobileGridGames.Droid
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {
        }

        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new CustomShellToolbarAppearanceTracker(this);
        }
    }

    public class CustomShellToolbarAppearanceTracker : ShellToolbarAppearanceTracker
    {
        public CustomShellToolbarAppearanceTracker(IShellContext context) : base(context)
        {
        }

        protected override void SetColors(AndroidX.AppCompat.Widget.Toolbar toolbar, IShellToolbarTracker toolbarTracker, Color foreground, Color background, Color title)
        {
            foreground = (Color)App.Current.Resources["GameTitleBarTextColor"];
            background = (Color)App.Current.Resources["Primary"];

            base.SetColors(toolbar, toolbarTracker, foreground, background, title);

            // The NavigationContentDescription get reset back to its default of "OK"
            // whenever a game is selected from the flyout. As such, don't set a name
            // of "Menu" here which isn't permanent.
            //toolbar.NavigationContentDescription = "Menu";
        }
    }
}