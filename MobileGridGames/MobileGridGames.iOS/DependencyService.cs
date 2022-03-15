using Foundation;
using MobileGridGames.Services;
using System;
using System.Linq;
using UIKit;

using MobileCoreServices;
using System.Threading.Tasks;
using System.IO;
using MobileGridGames.ViewModels;
using CoreFoundation;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(MobileGridGames.iOS.MobileGridGamesPlatformAction))]
namespace MobileGridGames.iOS
{
    public class MobileGridGamesPlatformAction : IMobileGridGamesPlatformAction
    {
        private System.Timers.Timer screenReaderAnnouncementTimer;
        private string nextScreenReaderAnnouncement = "";

        public void ScreenReaderAnnouncement(string notification)
        {
            // Take the action described at:
            // https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/accessibility

            if (UIAccessibility.IsVoiceOverRunning)
            {
                // Use a timer here to reduce the chances of a newly focused element announcement
                // interrupting this custom announcement.
                if (screenReaderAnnouncementTimer == null)
                {
                    screenReaderAnnouncementTimer = new System.Timers.Timer(250);

                    screenReaderAnnouncementTimer.Elapsed += this.OnTimerElapsed;
                    screenReaderAnnouncementTimer.AutoReset = false;
                }

                nextScreenReaderAnnouncement = notification;

                screenReaderAnnouncementTimer.Start();
            }
        }

        private void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string notification = nextScreenReaderAnnouncement;

            UIAccessibility.PostNotification(
              UIAccessibilityPostNotification.Announcement,
                new NSString(notification));

        }

        // Return a file that exists in the folder containing a set of custom pictures.
        public Task<string> GetPairsPictureFolder()
        {
            string result = "";

            var tcs = new TaskCompletionSource<string>();

            try
            {
                // Ask the player to select a folder.
                var docPicker = new UIDocumentPickerViewController(
                    new string[] { UTType.Folder }, UIDocumentPickerMode.Open);

                var currentViewController = GetCurrentUIController();
                if (currentViewController != null)
                {
                    currentViewController.PresentViewController(docPicker, true, null);
                }

                docPicker.WasCancelled += (sender, wasCancelledArgs) =>
                {
                    tcs.SetResult("");
                };

                docPicker.DidPickDocumentAtUrls += (object sender, UIDocumentPickedAtUrlsEventArgs e) =>
                {
                    Console.WriteLine("url = {0}", e.Urls[0].AbsoluteString);

                    // Wrap all file/folder access here in Start/StopAccessingSecurityScopedResource.
                    var start = e.Urls[0].StartAccessingSecurityScopedResource();

                    // Copy all the files of interest to a dedicated folder beneath the app's temp folder.
                    var targetFolder = Path.Combine(Path.GetTempPath(), "PairsGameCurrentPictures");

                    // First delete any temporary folder we may have created earlier.
                    if (Directory.Exists(targetFolder))
                    {
                        Directory.Delete(targetFolder, true);
                    }

                    // Now create a new temporary folder to use.
                    targetFolder = Path.Combine(targetFolder, e.Urls[0].LastPathComponent);
                    Directory.CreateDirectory(targetFolder);

                    // Now enumerate the folder selected by the player.
                    var filePathUrl = e.Urls[0].FilePathUrl;

                    NSError err;
                    var selectedContent = NSFileManager.DefaultManager.GetDirectoryContent(
                        e.Urls[0],
                        null,
                        NSDirectoryEnumerationOptions.SkipsHiddenFiles,
                        out err);

                    int imageCount = 0;
                    for (int i = 0; i < selectedContent.Count(); ++i)
                    {
                        Console.WriteLine("File = {0}", selectedContent[i].AbsoluteString);

                        var filename = selectedContent[i].LastPathComponent;
                        var targetFilename = Path.Combine(targetFolder, filename);
                        var targetFilenameUrl = NSUrl.FromFilename(targetFilename);

                        var fileManager = new NSFileManager();
                        fileManager.Copy(selectedContent[i], targetFilenameUrl, out err);

                        var item = new PictureData();
                        item.Index = imageCount + 1;
                        item.FullPath = targetFilename;
                        item.FileName = filename;

                        ++imageCount;

                        if (selectedContent[i].PathExtension == "txt")
                        {
                            result = targetFilename;
                        }
                    }

                    e.Urls[0].StopAccessingSecurityScopedResource();

                    tcs.SetResult(result);
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine("url = {0}", ex.Message);

                tcs.SetResult("");
            }

            return tcs.Task;
        }

        // Make sure the app is ready to present the iOS folder picker.
        public UIViewController GetCurrentUIController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
            {
                return null;
            }

            if (window.RootViewController.PresentedViewController == null)
            {
                window = UIApplication.SharedApplication.Windows
                         .First(i => i.RootViewController != null &&
                                     i.RootViewController.GetType().FullName
                                     .Contains(typeof(Xamarin.Forms.Platform.iOS.Platform).FullName));
            }

            UIViewController viewController = window.RootViewController;

            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            return viewController;
        }
    }
}
