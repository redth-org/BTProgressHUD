using System;
using MonoTouch.UIKit;

namespace BigTed
{
	public static class BTProgHUD
	{
		public static void Show (string status = null, float progress = -1, BTProgressHUD.MaskType maskType = BTProgressHUD.MaskType.None)
		{
			BTProgressHUD.Shared.Show (status, progress, maskType);
		}

		public static void Show (string cancelCaption, Action cancelCallback, string status = null, float progress = -1, BTProgressHUD.MaskType maskType = BTProgressHUD.MaskType.None)
		{
			BTProgressHUD.Shared.Show (cancelCaption, cancelCallback, status, progress, maskType);
		}


		public static void ShowToast(string status, bool showToastCentered = true, double timeoutMs = 1000)
		{
			BTProgressHUD.Shared.ShowToast (status, showToastCentered, timeoutMs);
		}
		public static void SetStatus (string status)
		{
			BTProgressHUD.Shared.SetStatus (status);
		}

		public static void ShowSuccessWithStatus(string status, double timeoutMs = 1000)
		{
			BTProgressHUD.Shared.ShowSuccessWithStatus (status, timeoutMs);
		}

		public static void ShowErrorWithStatus(string status, double timeoutMs = 1000)
		{
			BTProgressHUD.Shared.ShowErrorWithStatus (status, timeoutMs);
		}
		public static void ShowImage(UIImage image, string status, double timeoutMs = 1000) 
		{
			BTProgressHUD.Shared.ShowImage (image, status, timeoutMs);
		}

		public static void Dismiss()
		{
			BTProgressHUD.Shared.Dismiss ();
		}
		public static bool IsVisible
		{
			get 
			{
				return BTProgressHUD.Shared.IsVisible;
			}
		}


	}
}

