using System;
using UIKit;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using Foundation;


/*
 * The source for this is on
 * https://github.com/nicwise/BTProgressHUD
 */
using System.Threading.Tasks;
using BTProgressHUD;

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

            MakeButton("Run first - off main thread", () =>
            {
                //this must be the first one to run.
                // once BTProgressHUD.ANTYTHING has been called once on the UI thread, 
                // it'll be setup. So this is an initial call OFF the main thread.
                // Should except in debug.
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        BTProgressHUD.BTProgressHUD.Show(); 
                        KillAfter();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
            });

            MakeButton("Show", () =>
            {
                BTProgressHUD.BTProgressHUD.Show(); 
                KillAfter();
            });

            MakeButton("Cancel problem 3", () =>
                BTProgressHUD.BTProgressHUD.Show("Cancel", () => KillAfter(), "Cancel and text")
            );

            MakeButton("Cancel problem 2", () =>
                BTProgressHUD.BTProgressHUD.Show("Cancel", () => KillAfter())
            );

            MakeButton("Cancel problem", () =>
                BTProgressHUD.BTProgressHUD.Show("Cancel", () => KillAfter(), "This is a multilinetext\nSome more text\n more text\n and again more text")
            );

            MakeButton("Show Message", () =>
            {
                BTProgressHUD.BTProgressHUD.Show("Processing your image", -1, MaskType.Black); 
                KillAfter();
            });

            MakeButton("Show Success (Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.Outline);
            });

            MakeButton("Show Fail (Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.Outline);
            });
            
            MakeButton("Show Info (Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000,  ImageStyle.Outline);
            });
            
            MakeButton("Show Success (Full Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowSuccessWithStatus("Great success!", default, 1000, ImageStyle.OutlineFull);
            });

            MakeButton("Show Fail (Full Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowErrorWithStatus("Oh, thats bad", default, 1000, ImageStyle.OutlineFull);
            });
            
            MakeButton("Show Info (Full Outlined)", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowInfoWithStatus("Notice me!", default, 1000,  ImageStyle.OutlineFull);
            });

            MakeButton("Toast", () =>
            {
                BTProgressHUD.BTProgressHUD.ShowToast("Hello from the toast", false, 1000);
            });

            MakeButton("Dismiss", () =>
            {
                BTProgressHUD.BTProgressHUD.Dismiss(); 
            });

            MakeButton("Progress", () =>
            {
                progress = 0;
                BTProgressHUD.BTProgressHUD.Show("Hello!", progress);
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
                        BTProgressHUD.BTProgressHUD.Dismiss();
                    }
                    else
                    {
                        BTProgressHUD.BTProgressHUD.Show("Hello!", progress);
                    }
                });
                NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
            });

            MakeButton("Dismiss", () =>
            {
                BTProgressHUD.BTProgressHUD.Dismiss(); 
            });

            // This demo only works on iOS 13 and above because of UIImage.GetSystemImage
            // But it should work with versions below if you just use a UIImage
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                MakeButton("Success With Status", () =>
                    BTProgressHUD.BTProgressHUD.ShowImage(UIImage.GetSystemImage("photo"),"Success with image", MaskType.Black, 4000f)
                );
            }
        }

        void KillAfter(float timeout = 5)
        {
            if (timer != null)
            {
                timer.Invalidate();
            }
            timer = NSTimer.CreateRepeatingTimer(timeout, delegate
            {
                BTProgressHUD.BTProgressHUD.Dismiss();
            });
            NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
        }

        UIButton previousButton;

        void MakeButton(string text, Action del)
        {
            var button = new UIButton(UIButtonType.RoundedRect)
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            button.SetTitle(text, UIControlState.Normal);
            button.TouchUpInside += (o, e) =>
            {
                del();
            };
            View.Add(button);

            button.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

            if (previousButton != null)
            {
                button.TopAnchor.ConstraintEqualTo(previousButton.BottomAnchor, 10).Active = true;
            }
            else
            {
                button.TopAnchor.ConstraintEqualTo(View.SafeAreaLayoutGuide.TopAnchor).Active = true;
            }

            previousButton = button;
        }
    }
}

