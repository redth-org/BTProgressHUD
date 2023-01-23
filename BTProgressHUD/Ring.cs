using System;
using UIKit;

namespace BigTed
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
        public double ProgressUpdateInterval = 300;

        public void ResetStyle(UIColor colorToUse)
        {
            Color = colorToUse;
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
            {
                BackgroundColor = UIColor.SystemBackground;
            }
            else
            {
                BackgroundColor = UIColor.White;
            }
            ProgressUpdateInterval = 300;
        }
    }
}

