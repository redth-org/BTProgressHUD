// BTProgressHUD - port of SVProgressHUD
//
//  https://github.com/nicwise/BTProgressHUD
// 
//  Ported by Nic Wise - 
//  Copyright 2013 Nic Wise. MIT license.
// 
//  SVProgressHUD.m
//
//  Created by Sam Vermette on 27.03.11.
//  Copyright 2011 Sam Vermette. All rights reserved.
//
//  https://github.com/samvermette/SVProgressHUD
//
//  Version 1.6.1

using System;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace BigTed
{
    public sealed class ProgressHUD : UIView
    {
        static readonly Class? ClsUIPeripheralHostView;
        static readonly Class? ClsUIKeyboard;
        static readonly Class? ClsUIInputSetContainerView;
        static readonly Class? ClsUIInputSetHostView;

        static readonly NSObject obj = new();

        UIImage? _errorImage;
        UIImage? _successImage;
        UIImage? _infoImage;
        UIImage? _errorOutlineImage;
        UIImage? _successOutlineImage;
        UIImage? _infoOutlineImage;
        UIImage? _errorOutlineFullImage;
        UIImage? _successOutlineFullImage;
        UIImage? _infoOutlineFullImage;

        MaskType _maskType;
        NSTimer? _fadeoutTimer;
        UIView? _overlayView;
        UIView? _hudView;
        UILabel? _stringLabel;
        UIImageView? _imageView;
        UIActivityIndicatorView? _spinnerView;
        UIButton? _cancelHud;
        NSTimer? _progressTimer;
        float _progress;
        CAShapeLayer? _backgroundRingLayer;
        CAShapeLayer? _ringLayer;
        List<NSObject>? _eventListeners;
        bool _displayContinuousImage;
        
        static ProgressHUD? _sharedHud;
        ToastPosition _toastPosition = ToastPosition.Center;

        static ProgressHUD()
        {
            //initialize static fields used for input view detection
            var ptrUIPeripheralHostView = Class.GetHandle("UIPeripheralHostView");
            if (ptrUIPeripheralHostView != IntPtr.Zero)
                ClsUIPeripheralHostView = new Class(ptrUIPeripheralHostView);
            var ptrUIKeyboard = Class.GetHandle("UIKeyboard");
            if (ptrUIKeyboard != IntPtr.Zero)
                ClsUIKeyboard = new Class(ptrUIKeyboard);
            var ptrUIInputSetContainerView = Class.GetHandle("UIInputSetContainerView");
            if (ptrUIInputSetContainerView != IntPtr.Zero)
                ClsUIInputSetContainerView = new Class(ptrUIInputSetContainerView);
            var ptrUIInputSetHostView = Class.GetHandle("UIInputSetHostView");
            if (ptrUIInputSetHostView != IntPtr.Zero)
                ClsUIInputSetHostView = new Class(ptrUIInputSetHostView);
        }

        public ProgressHUD() : this(UIScreen.MainScreen.Bounds)
        {
        }

        public ProgressHUD(CGRect frame) : base(frame)
        {
            UserInteractionEnabled = false;
            BackgroundColor = UIColor.Clear;
            Alpha = 0;
            AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            SetOSSpecificLookAndFeel();
        }

        public UIColor HudBackgroundColour { get; set; } = UIColor.FromWhiteAlpha(0.0f, 0.8f);
        public UIColor HudForegroundColor { get; set; } = UIColor.White;
        public UIColor HudStatusShadowColor { get; set; } = UIColor.Black;
        public UIColor HudToastBackgroundColor { get; set; } = UIColor.Clear;
        public UIFont HudFont { get; set; } = UIFont.BoldSystemFontOfSize(16f);
        public UITextAlignment HudTextAlignment { get; set; } = UITextAlignment.Center;
        public Ring Ring { get; } = new();

        public UIImage ErrorImage
        {
            get => _errorImage ?? ImageHelper.ErrorImage.Value;
            set => _errorImage = value;
        }

        public UIImage SuccessImage
        {
            get => _successImage ?? ImageHelper.SuccessImage.Value;
            set => _successImage = value;
        }

        public UIImage InfoImage
        {
            get => _infoImage ?? ImageHelper.InfoImage.Value;
            set => _infoImage = value;
        }

        public UIImage ErrorOutlineImage
        {
            get => _errorOutlineImage ?? ImageHelper.ErrorOutlineImage.Value;
            set => _errorOutlineImage = value;
        }

        public UIImage SuccessOutlineImage
        {
            get => _successOutlineImage ?? ImageHelper.SuccessOutlineImage.Value;
            set => _successOutlineImage = value;
        }

        public UIImage InfoOutlineImage
        {
            get => _infoOutlineImage ?? ImageHelper.InfoOutlineImage.Value;
            set => _infoOutlineImage = value;
        }

        public UIImage ErrorOutlineFullImage
        {
            get => _errorOutlineFullImage ?? ImageHelper.ErrorOutlineFullImage.Value;
            set => _errorOutlineFullImage = value;
        }

        public UIImage SuccessOutlineFullImage
        {
            get => _successOutlineFullImage ?? ImageHelper.SuccessOutlineFullImage.Value;
            set => _successOutlineFullImage = value;
        }

        public UIImage InfoOutlineFullImage
        {
            get => _infoOutlineFullImage ?? ImageHelper.InfoOutlineFullImage.Value;
            set => _infoOutlineFullImage = value;
        }

        public bool IsVisible => Alpha == 1;

        public static ProgressHUD Shared
        {
            get
            {
                if (_sharedHud == null)
                {
                    UIApplication.EnsureUIThread();
                    _sharedHud = new ProgressHUD(UIScreen.MainScreen.Bounds);
                }
                return _sharedHud;
            }
        }

        public float RingRadius { get; set; } = 14f;
        public float RingThickness { get; set; } = 6f;
        
        CAShapeLayer RingLayer
        {
            get
            {
                if (_ringLayer == null)
                {
                    var center = new CGPoint(HudView.Frame.Width / 2, HudView.Frame.Height / 2);
                    _ringLayer = ShapeHelper.CreateRingLayer(center, RingRadius, RingThickness, Ring.Color);
                    HudView.Layer.AddSublayer(_ringLayer);
                }
                return _ringLayer;
            }
        }

        CAShapeLayer? BackgroundRingLayer
        {
            get
            {
                if (_backgroundRingLayer == null)
                {
                    var center = new CGPoint(HudView.Frame.Width / 2, HudView.Frame.Height / 2);
                    _backgroundRingLayer = ShapeHelper.CreateRingLayer(center, RingRadius, RingThickness, Ring.BackgroundColor);
                    _backgroundRingLayer.StrokeEnd = 1;
                    HudView.Layer.AddSublayer(_backgroundRingLayer);
                }
                return _backgroundRingLayer;
            }
        }

        bool IsClear => _maskType is MaskType.Clear or MaskType.None;

        UIView OverlayView =>
            _overlayView ??= new UIView(UIScreen.MainScreen.Bounds)
            {
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight,
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = false,
                AccessibilityViewIsModal = true
            };

        UIView HudView
        {
            get
            {
                if (_hudView != null)
                    return _hudView;

                var hudView = new UIToolbar
                {
                    Translucent = true,
                    BarTintColor = HudBackgroundColour,
                    BackgroundColor = HudBackgroundColour,
                    AutoresizingMask =
                        UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleTopMargin |
                        UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin
                };
                hudView.Layer.CornerRadius = 10;
                hudView.Layer.MasksToBounds = true;

                AddSubview(hudView);

                hudView.LayoutIfNeeded();
                _hudView = hudView;
                return _hudView;
            }
        }

        UILabel StringLabel
        {
            get
            {
                _stringLabel ??= new UILabel
                {
                    BackgroundColor = HudToastBackgroundColor,
                    AdjustsFontSizeToFitWidth = true,
                    TextAlignment = HudTextAlignment,
                    BaselineAdjustment = UIBaselineAdjustment.AlignCenters,
                    TextColor = HudForegroundColor,
                    Font = HudFont,
                    Lines = 0
                };
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_stringLabel.Superview == null)
                {
                    HudView.AddSubview(_stringLabel);
                }
                return _stringLabel;
            }
        }

        UIButton CancelHudButton
        {
            get
            {
                if (_cancelHud == null)
                {
                    _cancelHud = new UIButton
                    {
                        BackgroundColor = UIColor.Clear,
                        UserInteractionEnabled = true,
                        Font = HudFont
                    };
                    
                    _cancelHud.SetTitleColor(HudForegroundColor, UIControlState.Normal);
                    UserInteractionEnabled = true;
                }
                
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_cancelHud.Superview == null)
                {
                    HudView.AddSubview(_cancelHud);
                    // Position the Cancel button at the bottom
                    /* var hudFrame = HudView.Frame;
                    var cancelFrame = _cancelHud.Frame;
                    var x = ((hudFrame.Width - cancelFrame.Width)/2) + 0;
                    var y = (hudFrame.Height - cancelFrame.Height - 10);
                    _cancelHud.Frame = new RectangleF(x, y, cancelFrame.Width, cancelFrame.Height);
                    HudView.SizeToFit();
                    */
                }
                return _cancelHud;
            }
        }

        UIImageView ImageView
        {
            get
            {
                _imageView ??= new UIImageView(new CGRect(0, 0, 32, 32))
                {
                    ContentMode = UIViewContentMode.ScaleAspectFill
                };
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_imageView.Superview == null)
                {
                    HudView.AddSubview(_imageView);
                }
                return _imageView;
            }
        }
        UIActivityIndicatorView SpinnerView
        {
            get
            {
                _spinnerView ??= new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge)
                {
                    HidesWhenStopped = true,
                    Bounds = new CGRect(0, 0, 37, 37),
                    Color = HudForegroundColor
                };

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (_spinnerView.Superview == null)
                    HudView.AddSubview(_spinnerView);

                return _spinnerView;
            }
        }

        float VisibleKeyboardHeight
        {
            get
            {
                foreach (var testWindow in UIApplication.SharedApplication.Windows)
                {
                    if (testWindow.Class.Handle != Class.GetHandle("UIWindow"))
                    {
                        foreach (var possibleKeyboard in testWindow.Subviews)
                        {
                            if ((ClsUIPeripheralHostView != null && possibleKeyboard.IsKindOfClass(ClsUIPeripheralHostView)) ||
                                (ClsUIKeyboard != null && possibleKeyboard.IsKindOfClass(ClsUIKeyboard)))
                            {
                                // Check that the keyboard is actually on screen
                                if (possibleKeyboard.Frame.IntersectsWith(testWindow.Frame))
                                    return (float)possibleKeyboard.Bounds.Size.Height;
                            }
                            else if (ClsUIInputSetContainerView != null && possibleKeyboard.IsKindOfClass(ClsUIInputSetContainerView))
                            {
                                foreach (var possibleKeyboardSubview in possibleKeyboard.Subviews)
                                {
                                    if (ClsUIInputSetHostView != null && possibleKeyboardSubview.IsKindOfClass(ClsUIInputSetHostView))
                                        // Check that the keyboard is actually on screen
                                        if (possibleKeyboardSubview.Frame.IntersectsWith(testWindow.Frame))
                                            return (float)possibleKeyboardSubview.Bounds.Size.Height;
                                }
                            }
                        }
                    }
                }

                return 0;
            }
        }

        private void SetOSSpecificLookAndFeel()
        {
            HudBackgroundColour = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? UIColor.SystemBackground.ColorWithAlpha(0.8f) : UIColor.White.ColorWithAlpha(0.8f);
            HudForegroundColor = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? UIColor.Label.ColorWithAlpha(0.8f) : UIColor.FromWhiteAlpha(0.0f, 0.8f);
            HudStatusShadowColor = UIDevice.CurrentDevice.CheckSystemVersion(13, 0) ? UIColor.Label.ColorWithAlpha(0.8f) : UIColor.FromWhiteAlpha(200f / 255f, 0.8f);
            RingThickness = 1f;
        }

        public void Show(string? status = null, float progress = -1, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            obj.InvokeOnMainThread(() => ShowProgressWorker(progress, status, maskType, timeoutMs: timeoutMs));
        }

        public void Show(string cancelCaption, Action cancelCallback, string? status = null,
                          float progress = -1, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            // Making cancelCaption optional hides the method via the overload
            if (string.IsNullOrEmpty(cancelCaption))
            {
                cancelCaption = "Cancel";
            }
            obj.InvokeOnMainThread(() => ShowProgressWorker(progress, status, maskType,
               cancelCaption: cancelCaption, cancelCallback: cancelCallback, timeoutMs: timeoutMs));
        }

        public void ShowContinuousProgress(string? status = null, MaskType maskType = MaskType.None, double timeoutMs = 1000, UIImage? img = null)
        {
            obj.InvokeOnMainThread(() => ShowProgressWorker(0, status, maskType, false, ToastPosition.Center, null, null, timeoutMs, true, img));
        }

        public void ShowContinuousProgressTest(string? status = null, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            obj.InvokeOnMainThread(() => ShowProgressWorker(0, status, maskType, false, ToastPosition.Center, null, null, timeoutMs, true));
        }

        public void ShowToast(string status, MaskType maskType = MaskType.None, ToastPosition toastPosition = ToastPosition.Center, double timeoutMs = 1000)
        {
            obj.InvokeOnMainThread(() => ShowProgressWorker(status: status, textOnly: true, toastPosition: toastPosition, timeoutMs: timeoutMs, maskType: maskType));
        }

        public void SetStatus(string status)
        {
            obj.InvokeOnMainThread(() => SetStatusWorker(status));
        }

        public void ShowSuccessWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            var image = imageStyle switch
            {
                ImageStyle.Default => SuccessImage,
                ImageStyle.Outline => SuccessOutlineImage,
                ImageStyle.OutlineFull => SuccessOutlineFullImage,
                _ => throw new ArgumentOutOfRangeException(nameof(imageStyle), imageStyle, "Use ImageStyle.Default, ImageStyle.Outline or ImageStyle.OutlineFull")
            };

            ShowImage(image, status, maskType, timeoutMs);
        }

        public void ShowErrorWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            var image = imageStyle switch
            {
                ImageStyle.Default => ErrorImage,
                ImageStyle.Outline => ErrorOutlineImage,
                ImageStyle.OutlineFull => ErrorOutlineFullImage,
                _ => throw new ArgumentOutOfRangeException(nameof(imageStyle), imageStyle, "Use ImageStyle.Default, ImageStyle.Outline or ImageStyle.OutlineFull")
            };

            ShowImage(image, status, maskType, timeoutMs);
        }

        public void ShowInfoWithStatus(string status, MaskType maskType = MaskType.None, double timeoutMs = 1000, ImageStyle imageStyle = ImageStyle.Default)
        {
            var image = imageStyle switch
            {
                ImageStyle.Default => InfoImage,
                ImageStyle.Outline => InfoOutlineImage,
                ImageStyle.OutlineFull => InfoOutlineFullImage,
                _ => throw new ArgumentOutOfRangeException(nameof(imageStyle), imageStyle, "Use ImageStyle.Default, ImageStyle.Outline or ImageStyle.OutlineFull")
            };

            ShowImage(image, status, maskType, timeoutMs);
        }

        public void ShowImage(UIImage image, string status, MaskType maskType = MaskType.None, double timeoutMs = 1000)
        {
            obj.InvokeOnMainThread(() => ShowImageWorker(image, status, maskType, TimeSpan.FromMilliseconds(timeoutMs)));
        }

        public void Dismiss()
        {
            obj.InvokeOnMainThread(DismissWorker);
        }

        public override void Draw(CGRect rect)
        {
            using var context = UIGraphics.GetCurrentContext();
            switch (_maskType)
            {
                case MaskType.Black:
                    UIColor.FromWhiteAlpha(0f, 0.5f).SetColor();
                    context.FillRect(Bounds);
                    break;
                case MaskType.Gradient:
                    var colors = new nfloat[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.75f };
                    var locations = new nfloat[] { 0.0f, 1.0f };
                    using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                    {
                        using (var gradient = new CGGradient(colorSpace, colors, locations))
                        {
                            var center = new CGPoint(Bounds.Size.Width / 2, Bounds.Size.Height / 2);
                            float radius = Math.Min((float)Bounds.Size.Width, (float)Bounds.Size.Height);
                            context.DrawRadialGradient(gradient, center, 0, center, radius, CGGradientDrawingOptions.DrawsAfterEndLocation);
                        }
                    }

                    break;
            }
        }

        void ShowProgressWorker(
            float progress = -1, string? status = null, MaskType maskType = MaskType.None, bool textOnly = false,
            ToastPosition toastPosition = ToastPosition.Center, string? cancelCaption = null, Action? cancelCallback = null,
            double timeoutMs = 1000, bool showContinuousProgress = false, UIImage? displayContinuousImage = null)
        {
            if (TintColor != null)
                Ring.ResetStyle(TintColor);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (OverlayView.Superview == null)
            {
                var window = GetActiveWindow();
                window?.AddSubview(OverlayView);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Superview == null)
                OverlayView.AddSubview(this);

            _fadeoutTimer = null;
            ImageView.Hidden = true;
            _maskType = maskType;
            _progress = progress;

            StringLabel.Text = status;

            if (!string.IsNullOrEmpty(cancelCaption))
            {
                CancelHudButton.SetTitle(cancelCaption, UIControlState.Normal);
                CancelHudButton.TouchUpInside += delegate
                {
                    Dismiss();
                    if (cancelCallback != null)
                    {
                        obj.InvokeOnMainThread(() => cancelCallback.DynamicInvoke(null));
                        //cancelCallback.DynamicInvoke(null);
                    }
                };
            }

            UpdatePosition(textOnly);

            if (showContinuousProgress)
            {
                if (displayContinuousImage != null)
                {
                    _displayContinuousImage = true;
                    ImageView.Image = displayContinuousImage;
                    ImageView.Hidden = false;
                }

                RingLayer.StrokeEnd = 0.0f;
                StartProgressTimer(TimeSpan.FromMilliseconds(Ring.ProgressUpdateInterval));
            }
            else
            {
                if (progress >= 0)
                {
                    ImageView.Image = null;
                    ImageView.Hidden = false;

                    SpinnerView.StopAnimating();
                    RingLayer.StrokeEnd = progress;
                }
                else if (textOnly)
                {
                    CancelRingLayerAnimation();
                    SpinnerView.StopAnimating();
                }
                else
                {
                    CancelRingLayerAnimation();
                    SpinnerView.StartAnimating();
                }
            }

            bool cancelButtonVisible = _cancelHud?.IsDescendantOfView(HudView) == true;

            // intercept user interaction with the underlying view
            if (maskType != MaskType.None || cancelButtonVisible)
            {
                OverlayView.UserInteractionEnabled = true;
            }
            else
            {
                OverlayView.UserInteractionEnabled = false;
            }

            OverlayView.Hidden = false;
            _toastPosition = toastPosition;
            PositionHUD(null);
            
            if (Alpha != 1)
            {
                RegisterNotifications();
                HudView.Transform.Scale(1.3f, 1.3f);

                if (IsClear)
                {
                    Alpha = 1f;
                    HudView.Alpha = 0f;
                }

                Animate(0.15f, 0,
                    UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState,
                    delegate
                    {
                        HudView.Transform.Scale(1f / 1.3f, 1f / 1.3f);
                        if (IsClear)
                        {
                            HudView.Alpha = 1f;
                        }
                        else
                        {
                            Alpha = 1f;
                        }
                    }, delegate
                    {
                        //UIAccessibilityPostNotification(UIAccessibilityScreenChangedNotification, string);

                        if (textOnly)
                            StartDismissTimer(TimeSpan.FromMilliseconds(timeoutMs));
                    });

                SetNeedsDisplay();
            }
        }

        void ShowImageWorker(UIImage image, string status, MaskType maskType, TimeSpan duration)
        {
            _progress = -1;
            CancelRingLayerAnimation();

            // this should happen when Dismiss is called, but it happens AFTER the animation ends
            // so sometimes, the cancel button is left on :(
            if (_cancelHud != null)
            {
                _cancelHud.RemoveFromSuperview();
                _cancelHud = null;
            }

            if (!IsVisible)
            {
                Show(null, -1F, maskType);
            }

            ImageView.TintColor = HudForegroundColor;
            ImageView.Image = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            ImageView.Hidden = false;
            StringLabel.Text = status;
            UpdatePosition();
            SpinnerView.StopAnimating();

            StartDismissTimer(duration);
        }

        void StartDismissTimer(TimeSpan duration)
        {
            _fadeoutTimer = NSTimer.CreateTimer(duration, _ => DismissWorker());
            NSRunLoop.Main.AddTimer(_fadeoutTimer, NSRunLoopMode.Common);
        }

        void StartProgressTimer(TimeSpan duration)
        {
            if (_progressTimer != null)
                return;

            _progressTimer = NSTimer.CreateRepeatingTimer(duration, _ => UpdateProgress());
            NSRunLoop.Current.AddTimer(_progressTimer, NSRunLoopMode.Common);
        }

        void StopProgressTimer()
        {
            _progressTimer?.Invalidate();
            _progressTimer = null;
        }

        void UpdateProgress()
        {
            obj.InvokeOnMainThread(delegate
            {
                if (!_displayContinuousImage)
                {
                    ImageView.Image = null;
                    ImageView.Hidden = false;
                }

                SpinnerView.StopAnimating();

                if (RingLayer.StrokeEnd > 1)
                {
                    RingLayer.StrokeEnd = 0.0f;
                }
                else
                {
                    RingLayer.StrokeEnd += 0.1f;
                }
            });
        }

        void CancelRingLayerAnimation()
        {
            CATransaction.Begin();
            CATransaction.DisableActions = true;
            HudView.Layer.RemoveAllAnimations();

            RingLayer.StrokeEnd = 0;
            if (RingLayer.SuperLayer != null)
            {
                RingLayer.RemoveFromSuperLayer();
            }
            _ringLayer = null;

            if (BackgroundRingLayer?.SuperLayer != null)
            {
                BackgroundRingLayer.RemoveFromSuperLayer();
            }
            _backgroundRingLayer = null;

            CATransaction.Commit();
        }

        void DismissWorker()
        {
            SetFadeoutTimer(null);
            SetProgressTimer(null);

            Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.AllowUserInteraction,
                delegate
                {
                    HudView.Transform.Scale(0.8f, 0.8f);
                    if (IsClear)
                    {
                        HudView.Alpha = 0f;
                    }
                    else
                    {
                        Alpha = 0f;
                    }
                }, delegate
                {
                    if (Alpha == 0f || HudView.Alpha == 0f)
                    {
                        InvokeOnMainThread(RemoveHud);
                    }
                });
        }

        void RemoveHud()
        {
            Alpha = 0f;
            HudView.Alpha = 0f;

            //Removing observers
            UnRegisterNotifications();
            NSNotificationCenter.DefaultCenter.RemoveObserver(this);

            if (TintColor != null)
                Ring.ResetStyle(TintColor);

            CancelRingLayerAnimation();
            StringLabel.RemoveFromSuperview();
            SpinnerView.RemoveFromSuperview();
            ImageView.RemoveFromSuperview();
            _cancelHud?.RemoveFromSuperview();

            _stringLabel = null;
            _spinnerView = null;
            _imageView = null;
            _cancelHud = null;

            HudView.RemoveFromSuperview();
            _hudView = null;
            OverlayView.RemoveFromSuperview();
            _overlayView = null;
            RemoveFromSuperview();

            GetActiveWindow()?.RootViewController?.SetNeedsStatusBarAppearanceUpdate();
        }

        void SetStatusWorker(string status)
        {
            StringLabel.Text = status;
            UpdatePosition();
        }

        void RegisterNotifications()
        {
            _eventListeners ??= new List<NSObject>();

            _eventListeners.Add(NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification,
                PositionHUD));
            _eventListeners.Add(NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification,
                PositionHUD));
            _eventListeners.Add(NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidHideNotification,
                PositionHUD));
            _eventListeners.Add(NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification,
                PositionHUD));
            _eventListeners.Add(NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification,
                PositionHUD));
        }

        void UnRegisterNotifications()
        {
            if (_eventListeners == null)
                return;

            NSNotificationCenter.DefaultCenter.RemoveObservers(_eventListeners);
            _eventListeners.Clear();
            _eventListeners = null;
        }

        void MoveToPoint(CGPoint newCenter, float angle)
        {
            HudView.Transform = CGAffineTransform.MakeRotation(angle);
            HudView.Center = newCenter;
        }

        void PositionHUD(NSNotification? notification)
        {
            double animationDuration = 0;

            Frame = UIScreen.MainScreen.Bounds;

            UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
            bool ignoreOrientation = UIDevice.CurrentDevice.CheckSystemVersion(8, 0);

            var keyboardHeight = GetKeyboardHeightFromNotification(notification, ignoreOrientation, orientation, ref animationDuration);

            CGRect orientationFrame = GetActiveWindow().Bounds;
            var activeHeight = GetActiveHeight(ignoreOrientation, orientation, orientationFrame, keyboardHeight);
            
            nfloat posY = (float)Math.Floor(activeHeight * 0.45); 
            nfloat posX = orientationFrame.Size.Width / 2f;
            nfloat textHeight = StringLabel.Frame.Height / 2f + 40;

            posY = _toastPosition switch
            {
                ToastPosition.Bottom => activeHeight - textHeight,
                ToastPosition.Top => textHeight,
                _ => posY
            };
            
            CGPoint newCenter;
            float rotateAngle;

            if (ignoreOrientation)
            {
                rotateAngle = 0.0f;
                newCenter = new CGPoint(posX, posY);
            }
            else
            {
                switch (orientation)
                {
                    case UIInterfaceOrientation.PortraitUpsideDown:
                        rotateAngle = (float)Math.PI;
                        newCenter = new CGPoint(posX, orientationFrame.Size.Height - posY);
                        break;
                    case UIInterfaceOrientation.LandscapeLeft:
                        rotateAngle = (float)(-Math.PI / 2.0f);
                        newCenter = new CGPoint(posY, posX);
                        break;
                    case UIInterfaceOrientation.LandscapeRight:
                        rotateAngle = (float)(Math.PI / 2.0f);
                        newCenter = new CGPoint(orientationFrame.Size.Height - posY, posX);
                        break;
                    default: // as UIInterfaceOrientationPortrait
                        rotateAngle = 0.0f;
                        newCenter = new CGPoint(posX, posY);
                        break;
                }
            }

            if (notification != null)
            {
                Animate(animationDuration,
                    0, UIViewAnimationOptions.AllowUserInteraction, delegate
                    {
                        MoveToPoint(newCenter, rotateAngle);
                    }, null);
            }
            else
            {
                MoveToPoint(newCenter, rotateAngle);
            }
        }

        private static nfloat GetActiveHeight(bool ignoreOrientation, UIInterfaceOrientation orientation,
            CGRect orientationFrame, nfloat keyboardHeight)
        {
            CGRect statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;

            if (!ignoreOrientation && IsLandscape(orientation))
            {
                orientationFrame.Size = new CGSize(orientationFrame.Size.Height, orientationFrame.Size.Width);
                statusBarFrame.Size = new CGSize(statusBarFrame.Size.Height, statusBarFrame.Size.Width);
            }

            var activeHeight = orientationFrame.Size.Height;

            if (keyboardHeight > 0)
                activeHeight += statusBarFrame.Size.Height * 2;

            activeHeight -= keyboardHeight;
            return activeHeight;
        }

        private nfloat GetKeyboardHeightFromNotification(NSNotification? notification, bool ignoreOrientation,
            UIInterfaceOrientation orientation, ref double animationDuration)
        {
            if (notification == null)
                return VisibleKeyboardHeight;
            
            nfloat keyboardHeight = 0;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(notification);
            animationDuration = UIKeyboard.AnimationDurationFromNotification(notification);

            if (notification.Name == UIKeyboard.WillShowNotification ||
                notification.Name == UIKeyboard.DidShowNotification)
            {
                if (ignoreOrientation || IsPortrait(orientation))
                    keyboardHeight = keyboardFrame.Size.Height;
                else
                    keyboardHeight = keyboardFrame.Size.Width;
            }

            return keyboardHeight;
        }

        void SetFadeoutTimer(NSTimer? newTimer)
        {
            if (_fadeoutTimer != null)
            {
                _fadeoutTimer.Invalidate();
                _fadeoutTimer = null;
            }

            if (newTimer != null)
                _fadeoutTimer = newTimer;
        }


        void SetProgressTimer(NSTimer? newTimer)
        {
            StopProgressTimer();

            if (newTimer != null)
                _progressTimer = newTimer;
        }

        void UpdatePosition(bool textOnly = false)
        {
            nfloat hudWidth = 100f;
            nfloat stringHeight = 0f;
            nfloat stringHeightBuffer = 20f;
            nfloat stringAndImageHeightBuffer = 80f;

            var labelRect = CGRect.Empty;

            string? text = StringLabel.Text;

            // False if it's text-only
            bool imageUsed = ImageView.Image != null || ImageView.Hidden;
            if (textOnly)
            {
                imageUsed = false;
            }

            nfloat hudHeight = GetDefaultHudHeight(textOnly, imageUsed, stringAndImageHeightBuffer, stringHeight, stringHeightBuffer);
            hudHeight = AdjustSizesForText(text, hudHeight, imageUsed, ref hudWidth, ref labelRect);

            // Adjust for Cancel Button
            string? cancelCaption = _cancelHud == null ? null : CancelHudButton.Title(UIControlState.Normal);
            if (!string.IsNullOrEmpty(cancelCaption))
            {
                const int gap = 20;

                var stringSize = new NSString(cancelCaption).GetBoundingRect(
                    new CGSize(200, 300),
                    NSStringDrawingOptions.UsesLineFragmentOrigin,
                    new UIStringAttributes { Font = StringLabel.Font },
                    null);

                var stringWidth = stringSize.Width;
                stringHeight = stringSize.Height;

                if (stringWidth > hudWidth)
                    hudWidth = (float)Math.Ceiling(stringWidth / 2) * 2;

                // Adjust for label
                nfloat cancelRectY;
                if (labelRect.Height > 0)
                {
                    cancelRectY = labelRect.Y + labelRect.Height + gap;
                }
                else
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        cancelRectY = 76;
                    }
                    else
                    {
                        cancelRectY = (imageUsed ? 66 : 9);
                    }
                }

                CGRect cancelRect;
                if (hudHeight > 100)
                {
                    cancelRect = new CGRect(12, cancelRectY, hudWidth, stringHeight);
                    labelRect = new CGRect(12, labelRect.Y, hudWidth, labelRect.Height);
                    hudWidth += 24;
                }
                else
                {
                    hudWidth += 24;
                    cancelRect = new CGRect(0, cancelRectY, hudWidth, stringHeight);
                    labelRect = new CGRect(0, labelRect.Y, hudWidth, labelRect.Height);
                }
                CancelHudButton.Frame = cancelRect;
                hudHeight += cancelRect.Height + (string.IsNullOrEmpty(text) ? 10 : gap);
            }

            HudView.Bounds = new CGRect(0, 0, hudWidth, hudHeight);
            if (!string.IsNullOrEmpty(text))
                ImageView.Center = new CGPoint(HudView.Bounds.Width / 2, 36);
            else
                ImageView.Center = new CGPoint(HudView.Bounds.Width / 2, HudView.Bounds.Height / 2);


            StringLabel.Hidden = false;
            StringLabel.Frame = labelRect;

            if (!textOnly)
            {
                if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(cancelCaption))
                {
                    SpinnerView.Center = new CGPoint((float)Math.Ceiling(HudView.Bounds.Width / 2.0f) + 0.5f, 40.5f);
                    if (_progress > -1 && BackgroundRingLayer != null)
                    {
                        BackgroundRingLayer.Position = RingLayer.Position = new CGPoint(HudView.Bounds.Width / 2, 36f);
                    }
                }
                else
                {
                    SpinnerView.Center = new CGPoint((float)Math.Ceiling(HudView.Bounds.Width / 2.0f) + 0.5f, (float)Math.Ceiling(HudView.Bounds.Height / 2.0f) + 0.5f);
                    if (_progress > -1 && BackgroundRingLayer != null)
                    {
                        BackgroundRingLayer.Position = RingLayer.Position = new CGPoint(HudView.Bounds.Width / 2, HudView.Bounds.Height / 2.0f + 0.5f);
                    }
                }
            }
        }

        private nfloat AdjustSizesForText(string? text, nfloat hudHeight, bool imageUsed, ref nfloat hudWidth,
            ref CGRect labelRect)
        {
            if (string.IsNullOrEmpty(text))
                return hudHeight;
            
            var lineCount = Math.Min(10, text!.Split('\n').Length + 1);

            var stringSize = new NSString(text).GetBoundingRect(new CGSize(200, 30 * lineCount),
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes { Font = StringLabel.Font },
                null);
            var stringWidth = stringSize.Width;
            var stringHeight = stringSize.Height;

            hudHeight += stringHeight;

            if (stringWidth > hudWidth)
                hudWidth = (float)Math.Ceiling(stringWidth / 2.0) * 2;

            float labelRectY = imageUsed ? 66 : 9;

            if (hudHeight > 100)
            {
                labelRect = new CGRect(12, labelRectY, hudWidth, stringHeight);
                hudWidth += 24;
            }
            else
            {
                hudWidth += 24;
                labelRect = new CGRect(0, labelRectY, hudWidth, stringHeight);
            }

            return hudHeight;
        }

        private static nfloat GetDefaultHudHeight(bool textOnly, bool imageUsed, nfloat stringAndImageHeightBuffer,
            nfloat stringHeight, nfloat stringHeightBuffer)
        {
            nfloat hudHeight;
            if (imageUsed)
            {
                hudHeight = stringAndImageHeightBuffer + stringHeight;
            }
            else
            {
                hudHeight = textOnly ? stringHeightBuffer : stringHeightBuffer + 40;
            }

            return hudHeight;
        }

        public static bool IsLandscape(UIInterfaceOrientation orientation) =>
            orientation is UIInterfaceOrientation.LandscapeLeft or UIInterfaceOrientation.LandscapeRight;

        public static bool IsPortrait(UIInterfaceOrientation orientation) =>
            orientation is UIInterfaceOrientation.Portrait or UIInterfaceOrientation.PortraitUpsideDown;

        private static UIWindow? GetActiveWindow()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                var scene = UIApplication.SharedApplication.ConnectedScenes.ToArray()
                    .OfType<UIWindowScene>()
                    .FirstOrDefault(s =>
                        s.ActivationState == UISceneActivationState.ForegroundActive || // scene in foreground or
                        s.Windows.Any(w => w.IsKeyWindow)); // current shown window

                if (scene != null)
                    return scene.Windows.FirstOrDefault(w => w.IsKeyWindow);
            }
            
            var windows = UIApplication.SharedApplication.Windows;
            var window = windows.LastOrDefault(w => w.WindowLevel == UIWindowLevel.Normal && !w.Hidden && w.IsKeyWindow);

            // As a last resort, use the first window.
            // In iOS 15, showing the HUD while the app is moving to the foreground sometimes
            // leads to this method getting called in a condition where
            // UIWindowScene.ActivationState == UISceneActivationStateForegroundInactive
            // and there is no window with IsKeyWindow == true
            if (window == null)
                window = windows.FirstOrDefault(w => w.WindowLevel == UIWindowLevel.Normal);

            return window ?? throw new Exception("Could not find active window");
        }
    }
}
