using System;
using System.Windows.Input;
using BigTed;
using Foundation;

namespace BTProgressHUDDemo2
{
    public class MainViewModel
    {
        float progress = -1;
        NSTimer timer;

        public MainViewModel()
        {
            this.Items = new[]
            {
                Create("Show", () => BTProgressHUD.Show(), true),
                Create("Cancel problem 3", () => BTProgressHUD.Show("Cancel", () => KillAfter(), "Cancel and text"), false),
                Create("Cancel problem 2", () => BTProgressHUD.Show("Cancel", () => KillAfter()), false),
            };

            //MakeButton("Cancel problem", () =>
            //    BTProgressHUD.Show("Cancel", () => KillAfter(), "This is a multilinetext\nSome more text\n more text\n and again more text"),
            //    stack
            //);

            //MakeButton("Show Message", () =>
            //{
            //    BTProgressHUD.Show("Processing your image", -1, MaskType.Black);
            //    KillAfter();
            //}, stack);

            //MakeButton("Show Success", () =>
            //{
            //    BTProgressHUD.ShowSuccessWithStatus("Great success!");
            //}, stack);

            //MakeButton("Show Fail", () =>
            //{
            //    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad");
            //}, stack);

            //MakeButton("Show Info", () =>
            //{
            //    BTProgressHUD.ShowInfoWithStatus("Notice me!");
            //}, stack);

            //MakeButton("Show Success (Outlined)", () =>
            //{
            //    BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.Outline);
            //}, stack);

            //MakeButton("Show Fail (Outlined)", () =>
            //{
            //    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.Outline);
            //}, stack);

            //MakeButton("Show Info (Outlined)", () =>
            //{
            //    BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000, ImageStyle.Outline);
            //}, stack);

            //MakeButton("Show Success (Full Outlined)", () =>
            //{
            //    BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.OutlineFull);
            //}, stack);

            //MakeButton("Show Fail (Full Outlined)", () =>
            //{
            //    BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.OutlineFull);
            //}, stack);

            //MakeButton("Show Info (Full Outlined)", () =>
            //{
            //    BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000, ImageStyle.OutlineFull);
            //}, stack);

            //MakeButton("Toast", () =>
            //{
            //    BTProgressHUD.ShowToast("Hello from the toast", false, 1000);
            //}, stack);

            //MakeButton("Dismiss", () =>
            //{
            //    BTProgressHUD.Dismiss();
            //}, stack);

            //MakeButton("Progress", () =>
            //{
            //    progress = 0;
            //    BTProgressHUD.Show("Hello!", progress);
            //    if (timer != null)
            //    {
            //        timer.Invalidate();
            //    }
            //    timer = NSTimer.CreateRepeatingTimer(0.5f, delegate
            //    {
            //        progress += 0.1f;
            //        if (progress > 1)
            //        {
            //            timer.Invalidate();
            //            timer = null;
            //            BTProgressHUD.Dismiss();
            //        }
            //        else
            //        {
            //            BTProgressHUD.Show("Hello!", progress);
            //        }
            //    });
            //    NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
            //}, stack);

            //MakeButton("Dismiss", () =>
            //{
            //    BTProgressHUD.Dismiss();
            //}, stack);

            //// This demo only works on iOS 13 and above because of UIImage.GetSystemImage
            //// But it should work with versions below if you just use a UIImage
            //if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            //{
            //    MakeButton("Success With Status", () =>
            //        BTProgressHUD.ShowImage(
            //            UIImage.GetSystemImage("photo"),
            //            "Success with image",
            //            MaskType.Black,
            //            4000f),
            //        stack
            //    );
            //}

            //MakeButton("Show modal", () =>
            //    NavigationController.PresentViewController(
            //        new UINavigationController(new ModalView())
            //        {
            //            ModalPresentationStyle = UIModalPresentationStyle.PageSheet
            //        }, true, null),
            //    stack
            //);
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