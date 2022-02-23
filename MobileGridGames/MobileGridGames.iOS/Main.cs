using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace MobileGridGames.iOS
{
    // Barker: The build warning below occurred when first building the project. So
    // I followed the advice and (manually) set the AutoGenerateBindingRedirects to
    // true in the iOS project file.
    //    Warning Found conflicts between different versions of the same dependent assembly.
    //    Please set the "AutoGenerateBindingRedirects" property to true in the project file.

    // Barker: I also get the warning below that others have hit. I'd add parentheses
    // to avoid the warning, but I don't know where they're meant to go. So let's hope 
    // things work anyway.
    //     The condition "'$(IsMacEnabled)' == 'true' And '$(IsAppExtension)' == 'true' Or
    //     '$(_PlatformName)' == 'macOS'" may have been evaluated incorrectly

    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.

            // Barker: VS created the code with the following line which leads to build warnings.
            //UIApplication.Main(args, null, "AppDelegate");

            // Barker: So replace that line with the following.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}
