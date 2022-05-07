using UIKit;

namespace BigTed
{
    public class Ring
    {
        /// <summary>
        /// Ring color
        /// </summary>
        public UIColor Color { get; set; } = UIColor.White;
        /// <summary>
        /// Background color
        /// </summary>
        public UIColor BackgroundColor { get; set; } = UIColor.DarkGray;
        /// <summary>
        /// Progress update interval in milliseconds
        /// </summary>
        public double ProgressUpdateInterval { get; set; } = 300;

        public void ResetStyle(UIColor colorToUse)
        {
            Color = colorToUse;
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
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

