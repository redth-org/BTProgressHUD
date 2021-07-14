using System;
using UIKit;
#if __UNIFIED__

#else
using MonoTouch.UIKit;
#endif

namespace BTProgressHUD
{
    public static class BTProgressHUD
    {
        public static void Show(string status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.Shared.Show(status, progress, maskType);
        }

        public static void Show(string cancelCaption, Action cancelCallback, string status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.Shared.Show(cancelCaption, cancelCallback, status, progress, maskType);
        }

        public static void ShowContinuousProgress(string status = null, MaskType maskType = MaskType.None)
        {
            ProgressHUD.Shared.ShowContinuousProgress(status, maskType);
        }

        public static void ShowToast(string status, bool showToastCentered = false, double timeoutMs = 1000)
        {
            ShowToast(status, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void ShowToast(string status, ToastPosition toastPosition = ToastPosition.Center, double timeoutMs = 1000)
        {
            ProgressHUD.Shared.ShowToast(status, MaskType.None, toastPosition, timeoutMs);
        }

        public static void ShowToast(string status, MaskType maskType = MaskType.None, bool showToastCentered = true, double timeoutMs = 1000)
        {
            ProgressHUD.Shared.ShowToast(status, maskType, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void SetStatus(string status)
        {
            ProgressHUD.Shared.SetStatus(status);
        }

        public static void ShowSuccessWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Outline)
        {
            ProgressHUD.Shared.ShowSuccessWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowErrorWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Outline)
        {
            ProgressHUD.Shared.ShowErrorWithStatus(status, maskType, timeoutMs, imageStyle);
        }
        
        public static void ShowInfoWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Outline)
        {
            ProgressHUD.Shared.ShowInfoWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowImage(UIImage image, string status, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            ProgressHUD.Shared.ShowImage(image, status, maskType, timeoutMs);
        }

        public static void Dismiss()
        {
            ProgressHUD.Shared.Dismiss();
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

