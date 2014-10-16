using System;
using MonoTouch.UIKit;
using BigTed;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Threading.Tasks;
using MonoTouch.Dialog;

namespace BTProgressHUDDemo
{
	public class MainViewController : DialogViewController
	{
		public MainViewController () : base(null, false)
		{

		}

		float progress = -1;
		NSTimer timer;
		UIAlertView alert;

		Section mainSection;

		public override void LoadView ()
		{
			base.LoadView ();

			var root = new RootElement ("BTProgressHUD");
			mainSection = new Section ();

			MakeButton ("Masks", () =>
			{
				ProgressHUD.Shared.Show("Loading...", -1, ProgressHUD.MaskType.Black, 0.75);
					KillAfter(5);
			});

			MakeButton ("Async", () =>
				{
					AsyncTest();
				});


			MakeButton ("Show Continuous Progress", () =>
			{
				ProgressHUD.Shared.Ring.Color = UIColor.Green;
                ProgressHUD.Shared.ShowContinuousProgress("Continuous progress...", ProgressHUD.MaskType.None, 1000);
				KillAfter (3);
			});

            MakeButton("Show Continuous Progress with Image", () =>
            {
                ProgressHUD.Shared.Ring.Color = UIColor.Green;
                ProgressHUD.Shared.ShowContinuousProgress("Continuous progress...", ProgressHUD.MaskType.None, 1000, UIImage.FromBundle("xamarin@2x.png"));
                KillAfter(3);
            });

			MakeButton ("Show", () => {
				ProgressHUD.Shared.Show (); 
				KillAfter ();
			});

			MakeButton ("Show BT", () => {
				BTProgressHUD.Show (); 
				if (timer != null)
				{
					timer.Invalidate ();
				}
				timer = NSTimer.CreateRepeatingTimer (2f, delegate
					{
						BTProgressHUD.Dismiss ();
						timer.Invalidate ();
						timer = null;
					});
				NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Common);
			});

			MakeButton ("Show with Cancel", () => {
				ProgressHUD.Shared.Show ("Cancel Me", () => {
					ProgressHUD.Shared.ShowErrorWithStatus ("Operation Cancelled!");
				}, "Please Wait"); 
				//KillAfter ();
			});

			MakeButton ("Show inside Alert", () => {
				alert = new UIAlertView ("Oh, Hai", "Press the button to show it.", null, "Cancel", "Show the HUD");

				alert.Clicked += (object sender, UIButtonEventArgs e) => {
					if (e.ButtonIndex == 0)
						return;
					ProgressHUD.Shared.Show ("this should never go away?");
					KillAfter ();
				};
				alert.Show ();
			});

			MakeButton ("Show Message", () => {
				ProgressHUD.Shared.Show (status: "Processing your image"); 
				KillAfter ();
			});

			MakeButton ("Show Success", () => {
				ProgressHUD.Shared.ShowSuccessWithStatus ("Great success!");
			});

			MakeButton ("Show Fail", () => {
				ProgressHUD.Shared.ShowErrorWithStatus ("Oh, thats bad");
			});

			MakeButton ("Show Fail 5 seconds", () => {
				ProgressHUD.Shared.ShowErrorWithStatus ("Oh, thats bad", timeoutMs: 5000);
			});

			MakeButton ("Toast Top", () => {
				//ProgressHUD.Shared.HudForegroundColor = UIColor.White;
				//ProgressHUD.Shared.HudToastBackgroundColor = UIColor.DarkGray;
				ProgressHUD.Shared.ShowToast ("Hello from the toast\nLine 2", toastPosition: ProgressHUD.ToastPosition.Top, timeoutMs: 3000);
			});

			MakeButton ("Toast Center", () => {
				//ProgressHUD.Shared.HudForegroundColor = UIColor.White;
				//ProgressHUD.Shared.HudToastBackgroundColor = UIColor.DarkGray;
				ProgressHUD.Shared.ShowToast ("Hello from the toast\r\nLine 2", toastPosition: ProgressHUD.ToastPosition.Center, timeoutMs: 3000);
			});

			MakeButton ("Toast Bottom", () => {
				//ProgressHUD.Shared.HudForegroundColor = UIColor.White;
				//ProgressHUD.Shared.HudToastBackgroundColor = UIColor.DarkGray;
				ProgressHUD.Shared.ShowToast ("Hello from the toast\r\nLine 2", toastPosition: ProgressHUD.ToastPosition.Bottom, timeoutMs: 3000);
			});

			MakeButton ("Progress", () => {
				progress = 0;
				ProgressHUD.Shared.Show ("Hello!", progress);
				if (timer != null)
				{
					timer.Invalidate ();
				}
				timer = NSTimer.CreateRepeatingTimer (0.5f, delegate
				{
					progress += 0.1f;
					if (progress > 1)
					{
						timer.Invalidate ();
						timer = null;
						ProgressHUD.Shared.Dismiss ();
					} else
					{
						ProgressHUD.Shared.Show ("Hello!", progress);
					}


				});
				NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Common);
			});

			MakeButton ("Dismiss", () => {
				ProgressHUD.Shared.Dismiss (); 

				if (timer != null) {
					timer.Invalidate();
					timer = null;
				}
			});

			//From a bug report from Jose
			MakeButton ("Show, Dismiss, remove cancel", () => {
				ShowWaitDismissWithProperCancelButton ();
			});

			root.Add (mainSection);

			Root = root;

		}

		async void AsyncTest()
		{
			ProgressHUD.Shared.Show("Logging in.");
			var res = await BackgroundSleepOperation();
			ProgressHUD.Shared.Dismiss();
		}

		async Task<bool> BackgroundSleepOperation ()
		{
			return await Task.Run (() => {
				Thread.Sleep (1);
				return true;
			});
		}



		async void ShowWaitDismissWithProperCancelButton ()
		{

			ProgressHUD.Shared.Show ("Cancel", delegate()
			{
				Console.WriteLine ("Canceled.");
			}, "Please wait", -1, ProgressHUD.MaskType.Black);

			var result = await BackgroundSleepOperation ();

			ProgressHUD.Shared.Dismiss ();

			ProgressHUD.Shared.ShowSuccessWithStatus ("Done", 2000);

		}



		void KillAfter (float timeout = 2)
		{
			if (timer != null)
			{
				timer.Invalidate ();
			}
			timer = NSTimer.CreateRepeatingTimer (timeout, delegate
			{
				ProgressHUD.Shared.Dismiss ();
				timer.Invalidate ();
				timer = null;
			});
			NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Common);
		}


		void MakeButton (string text, Action del)
		{
			var el = new StyledStringElement (text);
			el.Tapped += () => {
				del();
			};
			mainSection.Add (el);
		}
	}
}

