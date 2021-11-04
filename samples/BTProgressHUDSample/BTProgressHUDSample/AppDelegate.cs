using System.Threading.Tasks;
using BigTed;
using Foundation;
using UIKit;

namespace BTProgressHUDDemo
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        UINavigationController viewController;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

            // If you have defined a root view controller, set it here:
            // window.RootViewController = myViewController;

            viewController = new UINavigationController(new MainViewController());
			window.RootViewController = viewController;


			// make the window visible
			window.MakeKeyAndVisible ();
            
            BTProgressHUD.Show("Cancel", () => { }, "Startup");
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                BTProgressHUD.Dismiss();
            });
			
			return true;
		}
	}
}

