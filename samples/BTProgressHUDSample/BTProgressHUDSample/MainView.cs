using System;
using UIKit;
using BigTed;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using Foundation;


/*
 * The source for this is on
 * https://github.com/nicwise/BTProgressHUD
 */

namespace BTProgressHUDDemo
{
	public class MainViewController : UIViewController
	{
		public MainViewController ()
		{

		}
		UITextView text;
		float progress = -1;
		NSTimer timer;
		public override void LoadView ()
		{
			base.LoadView ();
			View.BackgroundColor = UIColor.LightGray;

			MakeButton ("Show", () => {
				BTProgressHUD.Show (); 
				KillAfter ();
			});

			MakeButton ("Show Message", () => {
				BTProgressHUD.Show ("Processing your image", -1, ProgressHUD.MaskType.Black); 
				KillAfter ();
			});

			MakeButton ("Show Success", () => {
				BTProgressHUD.ShowSuccessWithStatus("Great success!") ;
			});

			MakeButton ("Show Fail", () => {
				BTProgressHUD.ShowErrorWithStatus("Oh, thats bad") ;
			});

			MakeButton ("Toast", () => {
				BTProgressHUD.ShowToast("Hello from the toast", showToastCentered: false, timeoutMs:1000) ;

			});

			MakeButton("Toast bottom left", () =>
			{	
				BTProgressHUD.ShowToast("Hello from the toast bottom left", BigTed.ProgressHUD.ToastPosition.BottomLeft, timeoutMs: 1000);
			});

			MakeButton("Toast bottom right", () =>
			{
				BTProgressHUD.ShowToast("Hello from the toast bottom right", BigTed.ProgressHUD.ToastPosition.BottomRight, timeoutMs: 1000);
			});

			MakeButton("Toast center left", () =>
			{
				BTProgressHUD.ShowToast("Hello from the toast center left", BigTed.ProgressHUD.ToastPosition.CenterLeft, timeoutMs: 1000);
			});

			MakeButton("Toast center right", () =>
			{
				BTProgressHUD.ShowToast("Hello from the toast center right", BigTed.ProgressHUD.ToastPosition.CenterRight, timeoutMs: 1000);
			});

			MakeButton("Toast top left", () =>
			{
				BTProgressHUD.ShowToast("Hello from the toast top left", BigTed.ProgressHUD.ToastPosition.TopLeft, timeoutMs: 1000);
			});

			MakeButton("Toast top right", () =>
			{
				BTProgressHUD.ShowToast("Hello from the toast top right", BigTed.ProgressHUD.ToastPosition.TopRight, timeoutMs: 1000);
			});

			MakeButton ("Dismiss", () => {
				BTProgressHUD.Dismiss (); 
			});

			MakeButton ("Progress", () => {
				progress = 0;
				BTProgressHUD.Show("Hello!", progress);
				if (timer != null) 
				{
					timer.Invalidate();
				}
				timer = NSTimer.CreateRepeatingTimer(0.5f, delegate {
					progress += 0.1f;
					if (progress > 1)
					{
						timer.Invalidate();
						timer = null;
						BTProgressHUD.Dismiss();
					} else {
						BTProgressHUD.Show ("Hello!", progress);
					}


				});
				NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
			});

			MakeButton ("Dismiss", () => {
				BTProgressHUD.Dismiss (); 
			});

		}

		void KillAfter (float timeout = 1)
		{
			if (timer != null) 
			{
				timer.Invalidate();
			}
			timer = NSTimer.CreateRepeatingTimer(timeout, delegate {
				BTProgressHUD.Dismiss();
			});
			NSRunLoop.Current.AddTimer(timer, NSRunLoopMode.Common);
		}
		float y = 20;
		void MakeButton(string text, Action del)
		{
			float x = 20;

			var button = new UIButton (UIButtonType.RoundedRect);
			button.Frame = new RectangleF (x, y, 280, 40);
			button.SetTitle (text, UIControlState.Normal);
			button.TouchUpInside += (o,e) => {
				del() ;
			};
			View.Add (button);
		
			
			y += 45;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}
	}
}

