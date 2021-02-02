using System.Linq;
using UIKit;

namespace BigTed {
    public static class UIApplicationExtensions{
        public static UIWindow GetKeyWindow(this UIApplication application) {
            return application.Windows.FirstOrDefault(window => window.IsKeyWindow) ?? application.Windows[0];
        }
    }
}
