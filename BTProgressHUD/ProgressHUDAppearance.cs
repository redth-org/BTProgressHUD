using System;
using UIKit;

namespace BigTed;

public static class ProgressHUDAppearance
{
    public static UIColor DefaultHudBackgroundColour { get; } = 
        OperatingSystem.IsIOSVersionAtLeast(13, 0) || OperatingSystem.IsMacCatalystVersionAtLeast(13) ?
            UIColor.SystemBackground.ColorWithAlpha(0.8f) :
            UIColor.White.ColorWithAlpha(0.8f);

    public static UIColor DefaultHudForegroundColor { get; } =
        OperatingSystem.IsIOSVersionAtLeast(13, 0) || OperatingSystem.IsMacCatalystVersionAtLeast(13) ?
            UIColor.Label.ColorWithAlpha(0.8f) :
            UIColor.FromWhiteAlpha(0.0f, 0.8f);

    public static UIColor DefaultHudToastBackgroundColor { get; } = UIColor.Clear;
    public static UIFont DefaultHudFont { get; } = UIFont.BoldSystemFontOfSize(16f);
    public static UITextAlignment DefaultHudTextAlignment { get; } = UITextAlignment.Center;
    public const float DefaultRingRadius = 14f;
    public const float DefaultRingThickness = 1f;
    
    public static float RingRadius { get; set; } = DefaultRingRadius;
    public static float RingThickness { get; set; } = DefaultRingThickness;
    
    
    public static UIColor HudBackgroundColour { get; set; } = DefaultHudBackgroundColour;
    public static UIColor HudForegroundColor { get; set; } = DefaultHudForegroundColor;
    public static UIColor HudToastBackgroundColor { get; set; } = DefaultHudToastBackgroundColor;
    public static UIFont HudFont { get; set; } = DefaultHudFont;
    public static UITextAlignment HudTextAlignment { get; set; } = DefaultHudTextAlignment;
    
    public static UIImage? ErrorImage { get; set; }
    public static UIImage? SuccessImage { get; set; }
    public static UIImage? InfoImage { get; set; }
    public static UIImage? ErrorOutlineImage { get; set; }
    public static UIImage? SuccessOutlineImage { get; set; }
    public static UIImage? InfoOutlineImage { get; set; }
    public static UIImage? ErrorOutlineFullImage { get; set; }
    public static UIImage? SuccessOutlineFullImage { get; set; }
    public static UIImage? InfoOutlineFullImage { get; set; }
}
