using BigTed;
using UIKit;

namespace BTProgressHUDDemo2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if IOS || MACCATALYST
        ProgressHUD.Initialize();
        ProgressHUDAppearance.HudFont = UIFont.PreferredHeadline;
        ProgressHUDAppearance.HudForegroundColor = UIColor.Purple;
#endif

        return builder.Build();
    }
}

