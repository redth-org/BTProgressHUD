/*
 * The source for this is on
 * https://github.com/nicwise/BTProgressHUD
 */

using System;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using BigTed;

namespace BTProgressHUDDemo
{
    public class MainViewController : UIViewController
    {
        public MainViewController()
        {

        }

        float progress = -1;
        NSTimer timer;

        public override void LoadView()
        {
            base.LoadView();
            View.BackgroundColor = UIColor.White;
            Title = "BTProgressHUD Samples";

            var scrollView = new UIScrollView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            Add(scrollView);

            scrollView.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            scrollView.LeadingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.LeadingAnchor).Active = true;
            scrollView.TrailingAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TrailingAnchor).Active = true;
            scrollView.BottomAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.BottomAnchor).Active = true;

            var stack = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Distribution = UIStackViewDistribution.EqualSpacing,
                Axis = UILayoutConstraintAxis.Vertical,
                Spacing = 10
            };

            scrollView.Add(stack);

            stack.TopAnchor.ConstraintEqualTo(scrollView.TopAnchor).Active = true;
            stack.CenterXAnchor.ConstraintEqualTo(scrollView.CenterXAnchor).Active = true;
            stack.BottomAnchor.ConstraintEqualTo(scrollView.BottomAnchor).Active = true;

            MakeButton("Run first - off main thread", () =>
            {
                //this must be the first one to run.
                // once BTProgressHUD.ANTYTHING has been called once on the UI thread, 
                // it'll be setup. So this is an initial call OFF the main thread.
                // Should except in debug.
                Task.Run(() =>
                {
                    try
                    {
                        BTProgressHUD.Show(); 
                        KillAfter();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
            }, stack);

            MakeButton("Show", () =>
            {
                BTProgressHUD.Show(); 
                KillAfter();
            }, stack);

            MakeButton("Cancel problem 3", () =>
                BTProgressHUD.Show("Cancel", () => KillAfter(), "Cancel and text"),
                stack
            );

            MakeButton("Cancel problem 2", () =>
                BTProgressHUD.Show("Cancel", () => KillAfter()),
                stack
            );

            MakeButton("Cancel problem", () =>
                BTProgressHUD.Show("Cancel", () => KillAfter(), "This is a multilinetext\nSome more text\n more text\n and again more text"),
                stack
            );

            MakeButton("Show Message", () =>
            {
                BTProgressHUD.Show("Processing your image", -1, MaskType.Black);
                KillAfter();
            }, stack);
            
            MakeButton("Show Success", () =>
            {
                BTProgressHUD.ShowSuccessWithStatus("Great success!");
            }, stack);

            MakeButton("Show Fail", () =>
            {
                BTProgressHUD.ShowErrorWithStatus("Oh, thats bad");
            }, stack);
            
            MakeButton("Show Info", () =>
            {
                BTProgressHUD.ShowInfoWithStatus("Notice me!");
            }, stack);

            MakeButton("Show Success (Outlined)", () =>
            {
                BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.Outline);
            }, stack);

            MakeButton("Show Fail (Outlined)", () =>
            {
                BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.Outline);
            }, stack);
            
            MakeButton("Show Info (Outlined)", () =>
            {
                BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000,  ImageStyle.Outline);
            }, stack);
            
            MakeButton("Show Success (Full Outlined)", () =>
            {
                BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.OutlineFull);
            }, stack);

            MakeButton("Show Fail (Full Outlined)", () =>
            {
                BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.OutlineFull);
            }, stack);
            
            MakeButton("Show Info (Full Outlined)", () =>
            {
                BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000,  ImageStyle.OutlineFull);
            }, stack);

            MakeButton("Toast", () =>
            {
                BTProgressHUD.ShowToast("Hello from the toast", false, 1000);
            }, stack);

            MakeButton("Dismiss", () =>
            {
                BTProgressHUD.Dismiss(); 
            }, stack);

            MakeButton("Progress", () =>
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
            }, stack);

            MakeButton("Dismiss", () =>
            {
                BTProgressHUD.Dismiss(); 
            }, stack);

            // This demo only works on iOS 13 and above because of UIImage.GetSystemImage
            // But it should work with versions below if you just use a UIImage
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                MakeButton("Success With Status", () =>
                    BTProgressHUD.ShowImage(
                        UIImage.GetSystemImage("photo"),
                        "Success with image", 
                        MaskType.Black, 
                        4000f), 
                    stack
                );
            }

            MakeButton("Show modal", () =>
                NavigationController.PresentViewController(
                    new UINavigationController(new ModalView())
                    {
                        ModalPresentationStyle = UIModalPresentationStyle.PageSheet 
                    }, true, null),
                stack
            );
        }

        void KillAfter(float timeout = 5)
        {
            if (timer != null)
            {
                timer.Invalidate();
            }
            timer = NSTimer.CreateRepeatingTimer(timeout, delegate
            {
                BTProgressHUD.Dismiss();
            });
            NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
        }

        void MakeButton(string text, Action del, UIStackView stack)
        {
            var button = new UIButton(UIButtonType.RoundedRect)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                HorizontalAlignment = UIControlContentHorizontalAlignment.Center,
            };
            button.SetTitle(text, UIControlState.Normal);
            button.TouchUpInside += (o, e) =>
            {
                del();
            };
            stack.AddArrangedSubview(button);

            button.HeightAnchor.ConstraintEqualTo(44).Active = true;
        }
    }
}

