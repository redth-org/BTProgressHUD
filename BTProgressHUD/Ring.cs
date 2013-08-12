using System;
using MonoTouch.UIKit;

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
        public Double ProgressUpdateInterval = 300;
    }
}
