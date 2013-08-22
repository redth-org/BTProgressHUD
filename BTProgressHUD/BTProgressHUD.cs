using System;
using MonoTouch.UIKit;

namespace BigTed
{
	public static class BTProgressHUD
	{
		public static void Show (string status = null, float progress = -1, ProgressHUD.MaskType maskType = ProgressHUD.MaskType.None)
		{
			ProgressHUD.Shared.Show (status, progress, maskType);
		}

		public static void Show (string cancelCaption, Action cancelCallback, string status = null, float progress = -1, ProgressHUD.MaskType maskType = ProgressHUD.MaskType.None)
		{
			ProgressHUD.Shared.Show (cancelCaption, cancelCallback, status, progress, maskType);
		}

		public static void ShowContinuousProgress (string status = null, ProgressHUD.MaskType maskType = ProgressHUD.MaskType.None)
		{
			ProgressHUD.Shared.ShowContinuousProgress (status, maskType);
		}

		public static void ShowToast(string status, bool showToastCentered = true, double timeoutMs = 1000)
		{
			ProgressHUD.Shared.ShowToast (status, showToastCentered, timeoutMs);
		}
		public static void SetStatus (string status)
		{
			ProgressHUD.Shared.SetStatus (status);
		}

		public static void ShowSuccessWithStatus(string status, double timeoutMs = 1000)
		{
			ProgressHUD.Shared.ShowSuccessWithStatus (status, timeoutMs);
		}

		public static void ShowErrorWithStatus(string status, double timeoutMs = 1000)
		{
			ProgressHUD.Shared.ShowErrorWithStatus (status, timeoutMs);
		}
		public static void ShowImage(UIImage image, string status, double timeoutMs = 1000) 
		{
			ProgressHUD.Shared.ShowImage (image, status, timeoutMs);
		}

		public static void Dismiss()
		{
			ProgressHUD.Shared.Dismiss ();
		}
		public static bool IsVisible
		{
			get 
			{
				return ProgressHUD.Shared.IsVisible;
			}
		}


	}
}

