using System;
using UIKit;

namespace BigTed;

public static class ProgressHUDAppearance
{
    public static UIColor DefaultHudBackgroundColor { get; } = 
        OperatingSystem.IsIOSVersionAtLeast(13, 0) || OperatingSystem.IsMacCatalystVersionAtLeast(13) ?
            UIColor.SystemBackground.ColorWithAlpha(0.8f) :
            UIColor.White.ColorWithAlpha(0.8f);

    public static UIColor DefaultForegroundColor { get; } =
        OperatingSystem.IsIOSVersionAtLeast(13, 0) || OperatingSystem.IsMacCatalystVersionAtLeast(13) ?
            UIColor.Label.ColorWithAlpha(0.8f) :
            UIColor.FromWhiteAlpha(0.0f, 0.8f);
    
    public static UIColor DefaultHudToastBackgroundColor { get; } = UIColor.Clear;
    public static UIFont DefaultHudFont { get; } = UIFont.BoldSystemFontOfSize(16f);
    public static UITextAlignment DefaultHudTextAlignment { get; } = UITextAlignment.Center;

    public static UIColor DefaultRingColor = UIColor.SystemBlue;

    public static UIColor DefaultRingBackgroundColor =
        OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13)
            ? UIColor.SystemBackground
            : DefaultHudBackgroundColor;

    public const float DefaultRingRadius = 14f;
    public const float DefaultRingThickness = 1f;
    public const float DefaultHudCornerRadius = 10f;
    public const double DefaultProgressUpdateInterval = 300;
    public const float DefaultActivityIndicatorScale = 1f;

    /// <summary>
    /// Get or set definite progress indicator ring radius to control its size
    /// </summary>
    public static float RingRadius { get; set; } = DefaultRingRadius;
    
    /// <summary>
    /// Get or set definite progress indicator ring stroke thickness
    /// </summary>
    public static float RingThickness { get; set; } = DefaultRingThickness;
    
    /// <summary>
    /// Get or set update interval for definite progress indicator ring animation
    /// </summary>
    public static double RingProgressUpdateInterval { get; set; } = DefaultProgressUpdateInterval;

    /// <summary>
    /// Get or set definite activity indicator scale to control its size
    /// </summary>
    public static float ActivityIndicatorScale { get; set; } = DefaultActivityIndicatorScale;

    /// <summary>
    /// Get or set definite progress indicator ring foreground color
    /// </summary>
    public static UIColor RingColor { get; set; } = DefaultRingColor;
    
    /// <summary>
    /// Get or set definite progress indicator ring background color
    /// </summary>
    public static UIColor RingBackgroundColor { get; set; } = DefaultRingBackgroundColor;

    /// <summary>
    /// Get or set hud corner radius
    /// </summary>
    public static float HudCornerRadius { get; set; } = DefaultHudCornerRadius;
    
    /// <summary>
    /// Get or set hud background color
    /// </summary>
    public static UIColor HudBackgroundColor { get; set; } = DefaultHudBackgroundColor;
    
    /// <summary>
    /// Get or set image tint color
    /// </summary>
    public static UIColor HudImageTintColor { get; set; } = DefaultForegroundColor;
    
    /// <summary>
    /// Get or set background color of toast
    /// </summary>
    public static UIColor HudToastBackgroundColor { get; set; } = DefaultHudToastBackgroundColor;
    
    /// <summary>
    /// Get or set font used for texts shown in hud
    /// </summary>
    public static UIFont HudFont { get; set; } = DefaultHudFont;
    
    /// <summary>
    /// Get or set color of texts shown in hud
    /// </summary>
    public static UIColor HudTextColor { get; set; } = DefaultForegroundColor;
    
    /// <summary>
    /// Get or set font to use for cancel button
    /// </summary>
    public static UIFont HudButtonFont { get; set; } = DefaultHudFont;
    
    /// <summary>
    /// Get or set text color on cancel button
    /// </summary>
    public static UIColor HudButtonTextColor { get; set; } = DefaultForegroundColor;
    
    /// <summary>
    /// Get or set alignment on texts shown in hud
    /// </summary>
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

    public static void ResetToDefaults()
    {
        RingRadius = DefaultRingRadius;
        RingThickness = DefaultRingThickness;
        RingProgressUpdateInterval = DefaultProgressUpdateInterval;
        RingColor = DefaultRingColor;
        RingBackgroundColor = DefaultRingBackgroundColor;

        HudCornerRadius = DefaultHudCornerRadius;
        HudBackgroundColor = DefaultHudBackgroundColor;
        HudImageTintColor = DefaultForegroundColor;
        HudToastBackgroundColor = DefaultHudToastBackgroundColor;
        HudFont = DefaultHudFont;
        HudButtonFont = DefaultHudFont;
        HudTextColor = DefaultForegroundColor;
        HudButtonTextColor = DefaultForegroundColor;
        HudTextAlignment = DefaultHudTextAlignment;
    }
}
