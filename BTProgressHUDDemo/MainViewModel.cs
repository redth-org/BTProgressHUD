using System;
using System.Windows.Input;
using BigTed;
using Foundation;
using UIKit;

namespace BTProgressHUDDemo2
{
    public class MainViewModel
    {
        float progress = -1;
        NSTimer timer;

        public MainViewModel()
        {
            Items =
            [
                Create("Dismiss", () =>
                {
                    BTProgressHUD.Dismiss();
                }, false),
                Create("Show", () => { ProgressHUDAppearance.ActivityIndicatorScale = ProgressHUDAppearance.DefaultActivityIndicatorScale; BTProgressHUD.Show(); }, true),
                Create("Show with scaled activity indicator", () => { ProgressHUDAppearance.ActivityIndicatorScale = 2f; BTProgressHUD.Show(); }, true),
                Create("Cancel problem 3", () => BTProgressHUD.Show("Cancel", () => KillAfter(), "Cancel and text"), false),
                Create("Cancel problem 2", () => BTProgressHUD.Show("Cancel", () => KillAfter()), false),
                Create("Cancel problem", () => BTProgressHUD.Show("Cancel", () => KillAfter(), "This is a multilinetext\nSome more text\n more text\n and again more text"), false),
                Create("Show Message", () => 
                    BTProgressHUD.Show("Processing your image", 10, MaskType.Black), true),
                Create("Show Success", () =>
                {
                    BTProgressHUD.ShowSuccessWithStatus("Great success!");
                }, false),

                Create("Show Fail", () =>
                {
                    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad");
                }, false),

                Create("Show Info", () =>
                {
                    BTProgressHUD.ShowInfoWithStatus("Notice me!");
                }, false),

                Create("Show Success (Outlined)", () =>
                {
                    BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.Outline);
                }, false),

                Create("Show Fail (Outlined)", () =>
                {
                    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.Outline);
                }, false),

                Create("Show Info (Outlined)", () =>
                {
                    BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000, ImageStyle.Outline);
                }, false),

                Create("Show Success (Full Outlined)", () =>
                {
                    BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.OutlineFull);
                }, false),

                Create("Show Fail (Full Outlined)", () =>
                {
                    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.OutlineFull);
                }, false),

                Create("Show Info (Full Outlined)", () =>
                {
                    BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000, ImageStyle.OutlineFull);
                }, false),

                Create("Toast", () =>
                {
                    BTProgressHUD.ShowToast("Hello from the toast", false, 1000);
                }, false),
                Create("Progress", () =>
                {
                    progress = 0;
                    BTProgressHUD.Show("Hello!", progress);
                    if (timer != null)
                    {
                        timer.Invalidate();
                    }
                    timer = NSTimer.CreateRepeatingTimer(0.5f, delegate
                    {
                        progress += 0.1f;
                        if (progress > 1)
                        {
                            timer.Invalidate();
                            timer = null;
                            BTProgressHUD.Dismiss();
                        }
                        else
                        {
                            BTProgressHUD.Show("Hello!", progress);
                        }
                    });
                    NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
                }, false),
                Create("Customize", () =>
                {
                    ProgressHUDAppearance.RingColor = UIColor.Green;
                    ProgressHUDAppearance.RingBackgroundColor = UIColor.Brown;
                    
                    ProgressHUDAppearance.HudBackgroundColor = UIColor.Yellow;
                    ProgressHUDAppearance.HudTextColor = UIColor.Purple;
                    ProgressHUDAppearance.HudButtonTextColor = UIColor.Orange;
                    ProgressHUDAppearance.HudCornerRadius = 2;
                    ProgressHUDAppearance.HudTextAlignment = UITextAlignment.Left;
                    ProgressHUDAppearance.HudTextColor = UIColor.Cyan;
                    ProgressHUDAppearance.HudToastBackgroundColor = UIColor.Blue;
                }, false),
                Create("Reset Customization", ProgressHUDAppearance.ResetToDefaults, false)
            ];
        }


        public CommandItem[] Items { get; }

        CommandItem Create(string text, Action action, bool timedKill) => new CommandItem(
            text,
            new Command(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                if (timedKill)
                {
                    KillAfter();
                }
            })
        );

        void KillAfter()
        {
            if (timer != null)
            {
                timer.Invalidate();
            }
            timer = NSTimer.CreateRepeatingTimer(5, delegate
            {
                BTProgressHUD.Dismiss();
            });
            NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
        }
    }


    public record CommandItem(
        string Text,
        ICommand Command
    );
}
