using System;
using UIKit;

namespace BigTed
{
    // ReSharper disable once InconsistentNaming
    public static class BTProgressHUD
    {
        public static void Show(string? status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.ForDefaultWindow().Show(status, progress, maskType);
        }

        public static void Show(string cancelCaption, Action cancelCallback, string? status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.ForDefaultWindow().Show(cancelCaption, cancelCallback, status, progress, maskType);
        }

        public static void ShowContinuousProgress(string? status = null, MaskType maskType = MaskType.None)
        {
            ProgressHUD.ForDefaultWindow().ShowContinuousProgress(status, maskType);
        }

        public static void ShowToast(string status, bool showToastCentered = false, double timeoutMs = 1000)
        {
            ShowToast(status, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void ShowToast(string status, ToastPosition toastPosition = ToastPosition.Center, double timeoutMs = 1000)
        {
            ProgressHUD.ForDefaultWindow().ShowToast(status, MaskType.None, toastPosition, timeoutMs);
        }

        public static void ShowToast(string status, MaskType maskType = MaskType.None, bool showToastCentered = true, double timeoutMs = 1000)
        {
            ProgressHUD.ForDefaultWindow().ShowToast(status, maskType, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void SetStatus(string status)
        {
            ProgressHUD.ForDefaultWindow().SetStatus(status);
        }

        public static void ShowSuccessWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.ForDefaultWindow().ShowSuccessWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowErrorWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.ForDefaultWindow().ShowErrorWithStatus(status, maskType, timeoutMs, imageStyle);
        }
        
        public static void ShowInfoWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.ForDefaultWindow().ShowInfoWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowImage(UIImage image, string? status, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            ProgressHUD.ForDefaultWindow().ShowImage(image, status, maskType, timeoutMs);
        }

        public static void Dismiss()
        {
            ProgressHUD.ForDefaultWindow().Dismiss();
        }

        public static bool IsVisible
            => ProgressHUD.ForDefaultWindow().IsVisible;




        public static void Show(UIWindow forWindow, string? status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.For(forWindow).Show(status, progress, maskType);
        }

        public static void Show(UIWindow forWindow, string cancelCaption, Action cancelCallback, string? status = null, float progress = -1, MaskType maskType = MaskType.None)
        {
            ProgressHUD.For(forWindow).Show(cancelCaption, cancelCallback, status, progress, maskType);
        }

        public static void ShowContinuousProgress(UIWindow forWindow, string? status = null, MaskType maskType = MaskType.None)
        {
            ProgressHUD.For(forWindow).ShowContinuousProgress(status, maskType);
        }

        public static void ShowToast(UIWindow forWindow, string status, bool showToastCentered = false, double timeoutMs = 1000)
        {
            ShowToast(forWindow, status, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void ShowToast(UIWindow forWindow, string status, ToastPosition toastPosition = ToastPosition.Center, double timeoutMs = 1000)
        {
            ProgressHUD.For(forWindow).ShowToast(status, MaskType.None, toastPosition, timeoutMs);
        }

        public static void ShowToast(UIWindow forWindow, string status, MaskType maskType = MaskType.None, bool showToastCentered = true, double timeoutMs = 1000)
        {
            ProgressHUD.For(forWindow).ShowToast(status, maskType, showToastCentered ? ToastPosition.Center : ToastPosition.Bottom, timeoutMs);
        }

        public static void SetStatus(UIWindow forWindow, string status)
        {
            ProgressHUD.For(forWindow).SetStatus(status);
        }

        public static void ShowSuccessWithStatus(UIWindow forWindow, string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.For(forWindow).ShowSuccessWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowErrorWithStatus(UIWindow forWindow, string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.For(forWindow).ShowErrorWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowInfoWithStatus(UIWindow forWindow, string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            ProgressHUD.For(forWindow).ShowInfoWithStatus(status, maskType, timeoutMs, imageStyle);
        }

        public static void ShowImage(UIWindow forWindow, UIImage image, string? status, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            ProgressHUD.For(forWindow).ShowImage(image, status, maskType, timeoutMs);
        }

        public static void Dismiss(UIWindow forWindow)
        {
            ProgressHUD.For(forWindow).Dismiss();
        }

        public static bool GetIsVisible(UIWindow forWindow)
            => ProgressHUD.For(forWindow).IsVisible;
    }
}

