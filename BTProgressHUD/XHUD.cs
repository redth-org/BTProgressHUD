using System;
using BigTed;

namespace XHUD
{

	public enum MaskType
	{
		None = 1,
		Clear,
		Black,
		Gradient
	}



	public static class HUD
	{

		public static void Show(string message, int progress = -1, MaskType maskType = MaskType.Black)
		{
			BTProgressHUD.Show (message, (progress / 100), (ProgressHUD.MaskType)maskType);
		}

		public static void Dismiss()
		{
			BTProgressHUD.Dismiss ();
		}

		public static void ShowToast(string message, bool showToastCentered = true, double timeoutMs = 1000)
		{
			BTProgressHUD.ShowToast (message, showToastCentered, timeoutMs);
		}
	}
}
