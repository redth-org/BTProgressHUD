using System;
using UIKit;
#if __UNIFIED__

#else
using MonoTouch.UIKit;
#endif

namespace BTProgressHUD
{
    public class Ring
    {
        /// <summary>
        /// Ring color
        /// </summary>
        public UIColor Color = UIColor.White;
        /// <summary>
        /// Background color
        /// </summary>
        public UIColor BackgroundColor = UIColor.DarkGray;
        /// <summary>
        /// Progress update interval in milliseconds
        /// </summary>
        public Double ProgressUpdateInterval = 300;

        public void ResetStyle(UIColor colorToUse)
        {
            Color = colorToUse;
            if(UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                BackgroundColor = UIColor.SystemBackgroundColor;
            }
            else
            {
                BackgroundColor = UIColor.White;
            }
            ProgressUpdateInterval = 300;
        }
    }
}

