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
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreAnimation;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace BigTed
{
	public class ProgressHUD : UIView
	{
		public ProgressHUD () : this(UIScreen.MainScreen.Bounds)
		{
		}

		public ProgressHUD (RectangleF frame) : base(frame)
		{
			UserInteractionEnabled = false;
			BackgroundColor = UIColor.Clear;
			Alpha = 0;
			AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
		}

		public enum MaskType
		{
			None = 1,
			Clear,
			Black,
			Gradient
		}

		public UIColor HudBackgroundColour = UIColor.FromWhiteAlpha (0.0f, 0.8f);
		public UIColor HudForegroundColor = UIColor.White;
		public UIColor HudStatusShadowColor = UIColor.Black;
		public UIFont HudFont = UIFont.BoldSystemFontOfSize (16f);
		public Ring Ring = new Ring ();
		static NSObject obj = new NSObject ();

		public void Show (string status = null, float progress = -1, MaskType maskType = MaskType.None)
		{
			obj.InvokeOnMainThread (() => ShowProgressWorker (progress, status, maskType));
		}

		public void Show (string cancelCaption, Action cancelCallback, string status = null, float progress = -1, MaskType maskType = MaskType.None)
		{
			// Making cancelCaption optional hides the method via the overload
			if (string.IsNullOrEmpty (cancelCaption))
			{
				cancelCaption = "Cancel";
			}
			obj.InvokeOnMainThread (() => ShowProgressWorker (progress, status, maskType, cancelCaption: cancelCaption, cancelCallback: cancelCallback, timeoutMs: 1000));
		}

		public void ShowContinuousProgress (string status = null, MaskType maskType = MaskType.None)
		{
			obj.InvokeOnMainThread (() => ShowProgressWorker (0, status, maskType, false, true, null, null, 1000, true));
		}

		public void ShowToast (string status, bool showToastCentered = true, double timeoutMs = 1000)
		{
			obj.InvokeOnMainThread (() => ShowProgressWorker (status: status, textOnly: true, showToastCentered: showToastCentered, timeoutMs: timeoutMs));
		}

		public void SetStatus (string status)
		{
			obj.InvokeOnMainThread (() => SetStatusWorker (status));
		}

		public void ShowSuccessWithStatus (string status, double timeoutMs = 1000)
		{
			ShowImage (UIImage.FromBundle ("success.png"), status, timeoutMs);
		}

		public void ShowErrorWithStatus (string status, double timeoutMs = 1000)
		{
			ShowImage (UIImage.FromBundle ("error.png"), status, timeoutMs);
		}

		public void ShowImage (UIImage image, string status, double timeoutMs = 1000)
		{
			
			obj.InvokeOnMainThread (() => ShowImageWorker (image, status, TimeSpan.FromMilliseconds (timeoutMs)));
		}

		public void Dismiss ()
		{
			obj.InvokeOnMainThread (DismissWorker);
		}

		public bool IsVisible
		{
			get
			{
				return Alpha == 1;
			}
		}

		static ProgressHUD sharedHUD = null;

		public static ProgressHUD Shared
		{
			get
			{
				if (sharedHUD == null)
					sharedHUD = new ProgressHUD (UIScreen.MainScreen.Bounds);
				return sharedHUD;
			}
		}

		float _ringRadius = 14f;
		float _ringThickness = 6f;
		MaskType _maskType;
		NSTimer _fadeoutTimer;
		UIView _overlayView;
		UIView _hudView;
		UILabel _stringLabel;
		UIImageView _imageView;
		UIActivityIndicatorView _spinnerView;
		UIButton _cancelHud;
		NSTimer _progressTimer;
		float _progress;
		CAShapeLayer _backgroundRingLayer;
		CAShapeLayer _ringLayer;

		public override void Draw (RectangleF rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				switch (_maskType)
				{
					case MaskType.Black:
						UIColor.FromWhiteAlpha (0f, 0.5f).SetColor ();
						context.FillRect (Bounds);
						break;
					case MaskType.Gradient:
						float[] colors = new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.75f };
						float[] locations = new float[] { 0.0f, 1.0f };
						using (var colorSpace = CGColorSpace.CreateDeviceRGB())
						{
							using (var gradient = new CGGradient(colorSpace,colors, locations))
							{
								var center = new PointF (Bounds.Size.Width / 2, Bounds.Size.Height / 2);
								float radius = Math.Min (Bounds.Size.Width, Bounds.Size.Height);
								context.DrawRadialGradient (gradient, center, 0, center, radius, CGGradientDrawingOptions.DrawsAfterEndLocation);
							}
						}

						break;
				}
			}
		}
		/*
		void ShowProgressWorker(string cancelCaption, Delegate cancelCallback, float progress = -1, string status = null, MaskType maskType = MaskType.None){
			CancelHudButton.SetTitle(cancelCaption, UIControlState.Normal);
			CancelHudButton.TouchUpInside += delegate {
				BTProgressHUD.Dismiss();
				if(cancelCallback != null){
					cancelCallback.DynamicInvoke(null);
				}
			};
			UpdatePosition();
			ShowProgressWorker(progress, status, maskType);
		}
*/
		void ShowProgressWorker (float progress = -1, string status = null, MaskType maskType = MaskType.None, bool textOnly = false, 
		                         bool showToastCentered = true, string cancelCaption = null, Action cancelCallback = null, 
		                         double timeoutMs = 1000, bool showContinuousProgress = false)
		{
			if (OverlayView.Superview == null)
			{
				var windows = UIApplication.SharedApplication.Windows;
				Array.Reverse (windows);
				foreach (UIWindow window in windows)
				{
					if (window.WindowLevel == UIWindow.LevelNormal)
					{
						window.AddSubview (OverlayView);
					}
				}
			}

		
			if (Superview == null)
				OverlayView.AddSubview (this);
			
			_fadeoutTimer = null;
			ImageView.Hidden = true;
			_maskType = maskType;
			_progress = progress;
			
			StringLabel.Text = status;

			if (!string.IsNullOrEmpty (cancelCaption))
			{
				CancelHudButton.SetTitle (cancelCaption, UIControlState.Normal);
				CancelHudButton.TouchUpInside += delegate
				{
					Dismiss ();
					if (cancelCallback != null)
					{
						obj.InvokeOnMainThread (() => cancelCallback.DynamicInvoke (null));
						//cancelCallback.DynamicInvoke(null);
					}
				};
			}

			UpdatePosition (textOnly);

			if (showContinuousProgress)
			{
				RingLayer.StrokeEnd = 0.0f;
				StartProgressTimer (TimeSpan.FromMilliseconds (Ring.ProgressUpdateInterval));
			} else
			{
				if (progress >= 0)
				{
					ImageView.Image = null;
					ImageView.Hidden = false;
					SpinnerView.StopAnimating ();
					RingLayer.StrokeEnd = progress;
				} else if (textOnly)
				{
					CancelRingLayerAnimation ();
					SpinnerView.StopAnimating ();
				} else
				{
					CancelRingLayerAnimation ();
					SpinnerView.StartAnimating ();
				}
			}
			
			if (maskType != MaskType.None)
			{
				OverlayView.UserInteractionEnabled = true;
				//AccessibilityLabel = status;
				//IsAccessibilityElement = true;
			} else
			{
				OverlayView.UserInteractionEnabled = true;
				//hudView.IsAccessibilityElement = true;
			}

			OverlayView.Hidden = false;
			this.showToastCentered = showToastCentered;
			PositionHUD (null);

		
			if (Alpha != 1)
			{
				RegisterNotifications ();
				HudView.Transform.Scale (1.3f, 1.3f);

				UIView.Animate (0.15f, 0, 
				                UIViewAnimationOptions.AllowUserInteraction | UIViewAnimationOptions.CurveEaseOut | UIViewAnimationOptions.BeginFromCurrentState,
				                delegate
				{
					HudView.Transform.Scale ((float)1 / 1.3f, (float)1f / 1.3f);
					Alpha = 1;
				}, delegate
				{
					//UIAccessibilityPostNotification(UIAccessibilityScreenChangedNotification, string);

					if (textOnly)
						StartDismissTimer (TimeSpan.FromMilliseconds (timeoutMs));
				});

				SetNeedsDisplay ();
			}
		}

		void ShowImageWorker (UIImage image, string status, TimeSpan duration)
		{


			_progress = -1;
			CancelRingLayerAnimation ();

			//this should happen when Dismiss is called, but it happens AFTER the animation ends
			// so sometimes, the cancel button is left on :(
			if (CancelHudButton != null)
			{
				CancelHudButton.RemoveFromSuperview ();
				CancelHudButton = null;
			}

			if (!IsVisible)
				Show ();

			ImageView.Image = image;
			ImageView.Hidden = false;
			StringLabel.Text = status;
			UpdatePosition ();
			SpinnerView.StopAnimating ();

			StartDismissTimer (duration);
		}

		void StartDismissTimer (TimeSpan duration)
		{
			_fadeoutTimer = NSTimer.CreateTimer (duration, DismissWorker);
			NSRunLoop.Main.AddTimer (_fadeoutTimer, NSRunLoopMode.Common);
		}

		void StartProgressTimer (TimeSpan duration)
		{
			_progressTimer = NSTimer.CreateRepeatingTimer (duration, UpdateProgress);
			NSRunLoop.Current.AddTimer (_progressTimer, NSRunLoopMode.Common);
		}

		void UpdateProgress ()
		{
			obj.InvokeOnMainThread (delegate
			{
				ImageView.Image = null;
				ImageView.Hidden = false;
				SpinnerView.StopAnimating ();
		
				if (RingLayer.StrokeEnd > 1)
				{
					RingLayer.StrokeEnd = 0.0f;
				} else
				{
					RingLayer.StrokeEnd += 0.1f;
				}
			});
		}

		void CancelRingLayerAnimation ()
		{
			CATransaction.Begin ();
			CATransaction.DisableActions = true;
			HudView.Layer.RemoveAllAnimations ();
			
			RingLayer.StrokeEnd = 0;
			if (RingLayer.SuperLayer != null)
			{
				RingLayer.RemoveFromSuperLayer ();
			}
			RingLayer = null;
			
			if (BackgroundRingLayer.SuperLayer != null)
			{
				BackgroundRingLayer.RemoveFromSuperLayer ();
			}
			BackgroundRingLayer = null;
			
			CATransaction.Commit ();
		}

		CAShapeLayer RingLayer
		{
			get
			{
				if (_ringLayer == null)
				{
					var center = new PointF (HudView.Frame.Width / 2, HudView.Frame.Height / 2);
					_ringLayer = CreateRingLayer (center, _ringRadius, _ringThickness, Ring.Color);
					HudView.Layer.AddSublayer (_ringLayer);
				}
				return _ringLayer;
			}
			set { _ringLayer = value; }
		}

		CAShapeLayer BackgroundRingLayer
		{
			get
			{
				if (_backgroundRingLayer == null)
				{
					var center = new PointF (HudView.Frame.Width / 2, HudView.Frame.Height / 2);
					_backgroundRingLayer = CreateRingLayer (center, _ringRadius, _ringThickness, Ring.BackgroundColor);
					_backgroundRingLayer.StrokeEnd = 1;
					HudView.Layer.AddSublayer (_backgroundRingLayer);
				}
				return _backgroundRingLayer;
			}
			set { _backgroundRingLayer = value; }
		}

		PointF PointOnCircle (PointF center, float radius, float angleInDegrees)
		{
			float x = radius * (float)Math.Cos (angleInDegrees * Math.PI / 180) + radius;
			float y = radius * (float)Math.Sin (angleInDegrees * Math.PI / 180) + radius;
			return new PointF (x, y);
		}

		UIBezierPath CreateCirclePath (PointF center, float radius, int sampleCount)
		{
			var smoothedPath = new UIBezierPath ();
			PointF startPoint = PointOnCircle (center, radius, -90);

			smoothedPath.MoveTo (startPoint);

			float delta = 360 / sampleCount;
			float angleInDegrees = -90;
			for (int i = 1; i < sampleCount; i++)
			{
				angleInDegrees += delta;
				var point = PointOnCircle (center, radius, angleInDegrees);
				smoothedPath.AddLineTo (point);
			}
			smoothedPath.AddLineTo (startPoint);
			return smoothedPath;
		}

		CAShapeLayer CreateRingLayer (PointF center, float radius, float lineWidth, UIColor color)
		{
			var smoothedPath = CreateCirclePath (center, radius, 72);
			var slice = new CAShapeLayer ();
			slice.Frame = new RectangleF (center.X - radius, center.Y - radius, radius * 2, radius * 2);
			slice.FillColor = UIColor.Clear.CGColor;
			slice.StrokeColor = color.CGColor;
			slice.LineWidth = lineWidth;
			slice.LineCap = CAShapeLayer.JoinBevel;
			slice.LineJoin = CAShapeLayer.JoinBevel;
			slice.Path = smoothedPath.CGPath;
			return slice;
		
		}

		UIView OverlayView
		{
			get
			{
				if (_overlayView == null)
				{
					_overlayView = new UIView (UIScreen.MainScreen.Bounds);
					_overlayView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
					_overlayView.BackgroundColor = UIColor.Clear;
					_overlayView.UserInteractionEnabled = false;
				}
				return _overlayView;
			}
			set { _overlayView = value; }
		}

		UIView HudView
		{
			get
			{
				if (_hudView == null)
				{
					_hudView = new UIView ();
					_hudView.Layer.CornerRadius = 10;
					_hudView.BackgroundColor = HudBackgroundColour;
					_hudView.AutoresizingMask = (UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleTopMargin |
						UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleLeftMargin);
					AddSubview (_hudView);
				}
				return _hudView;
			}
			set { _hudView = value; }
		}

		UILabel StringLabel
		{
			get
			{
				if (_stringLabel == null)
				{
					_stringLabel = new UILabel ();
					_stringLabel.BackgroundColor = UIColor.Clear;
					_stringLabel.AdjustsFontSizeToFitWidth = true;
					_stringLabel.TextAlignment = UITextAlignment.Center;
					_stringLabel.BaselineAdjustment = UIBaselineAdjustment.AlignCenters;
					_stringLabel.TextColor = HudForegroundColor;
					_stringLabel.Font = HudFont;
					_stringLabel.ShadowColor = HudStatusShadowColor;
					_stringLabel.ShadowOffset = new SizeF (0, -1);
					_stringLabel.Lines = 0;
				}
				if (_stringLabel.Superview == null)
				{
					HudView.AddSubview (_stringLabel);
				}
				return _stringLabel;
			}
			set { _stringLabel = value; }
		}

		UIButton CancelHudButton
		{
			get
			{
				if (_cancelHud == null)
				{
					_cancelHud = new UIButton ();

					_cancelHud.BackgroundColor = UIColor.Clear;
					_cancelHud.SetTitleColor (HudForegroundColor, UIControlState.Normal);
					_cancelHud.UserInteractionEnabled = true;
					_cancelHud.Font = HudFont;
					this.UserInteractionEnabled = true; 
				}
				if (_cancelHud.Superview == null)
				{
					HudView.AddSubview (_cancelHud);
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
			set
			{
				_cancelHud = value;
			}
		}

		UIImageView ImageView
		{
			get
			{
				if (_imageView == null)
				{
					_imageView = new UIImageView (new RectangleF (0, 0, 28, 28));
				}
				if (_imageView.Superview == null)
				{
					HudView.AddSubview (_imageView);
				}
				return _imageView;
			}
			set { _imageView = value; }
		}

		UIActivityIndicatorView SpinnerView
		{
			get
			{
				if (_spinnerView == null)
				{
					_spinnerView = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
					_spinnerView.HidesWhenStopped = true;
					_spinnerView.Bounds = new RectangleF (0, 0, 37, 37);
					_spinnerView.Color = HudForegroundColor;
				}

				if (_spinnerView.Superview == null)
					HudView.AddSubview (_spinnerView);

				return _spinnerView;
			}
			set { _spinnerView = value; }
		}

		float VisibleKeyboardHeight
		{
			get
			{
				UIWindow keyboardWindow = null;
				foreach (var testWindow in UIApplication.SharedApplication.Windows)
				{
					if (!(testWindow is UIWindow))
					{
						keyboardWindow = testWindow;
						break;
					}
				}

				if (keyboardWindow == null)
					return 0;

				foreach (var possibleKeyboard in keyboardWindow.Subviews)
				{
					if (possibleKeyboard.GetType ().Name == "UIPeripheralHostView" ||
						possibleKeyboard.GetType ().Name == "UIKeyboard")
					{
						return possibleKeyboard.Bounds.Size.Height;
					}
				}

				return 0;
			}
		}

		void DismissWorker ()
		{
			SetFadeoutTimer (null);
			SetProgressTimer (null);

			UIView.Animate (0.3, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.AllowUserInteraction,
			                delegate
			{
				HudView.Transform.Scale (0.8f, 0.8f);
				this.Alpha = 0;
			}, delegate
			{
				if (Alpha == 0)
				{
					InvokeOnMainThread (delegate
					{
						NSNotificationCenter.DefaultCenter.RemoveObserver (this);

						Ring.ResetStyle();

						CancelRingLayerAnimation ();
						StringLabel.RemoveFromSuperview ();
						SpinnerView.RemoveFromSuperview ();
						ImageView.RemoveFromSuperview ();
						if (CancelHudButton != null) 
							CancelHudButton.RemoveFromSuperview ();

						StringLabel = null;
						SpinnerView = null;
						ImageView = null;
						CancelHudButton = null;

						HudView.RemoveFromSuperview ();
						HudView = null;
						OverlayView.RemoveFromSuperview ();
						OverlayView = null;
						this.RemoveFromSuperview ();
					});
				}
			});
		}

		void SetStatusWorker (string status)
		{
			StringLabel.Text = status;
			UpdatePosition ();

		}

		void RegisterNotifications ()
		{

			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidChangeStatusBarOrientationNotification,
			                                                PositionHUD);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillHideNotification,
			                                                PositionHUD);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.DidHideNotification,
			                                                PositionHUD);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification,
			                                                PositionHUD);
			NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.DidShowNotification,
			                                                PositionHUD);
		}

		void MoveToPoint (PointF newCenter, float angle)
		{
			HudView.Transform = CGAffineTransform.MakeRotation (angle); 
			HudView.Center = newCenter;
		}

		bool showToastCentered = true;

		void PositionHUD (NSNotification notification)
		{
			float keyboardHeight = 0;
			double animationDuration = 0;

			UIInterfaceOrientation orientation = UIApplication.SharedApplication.StatusBarOrientation;
			
			if (notification != null)
			{
				RectangleF keyboardFrame = UIKeyboard.FrameEndFromNotification (notification);
				animationDuration = UIKeyboard.AnimationDurationFromNotification (notification);
				
				if (notification.Name == UIKeyboard.WillShowNotification || notification.Name == UIKeyboard.DidShowNotification)
				{
					if (IsPortrait (orientation))
						keyboardHeight = keyboardFrame.Size.Height;
					else
						keyboardHeight = keyboardFrame.Size.Width;
				} else
				
					keyboardHeight = 0;

			} else
			{
				keyboardHeight = VisibleKeyboardHeight;
			}
			
			RectangleF orientationFrame = UIScreen.MainScreen.Bounds;
			RectangleF statusBarFrame = UIApplication.SharedApplication.StatusBarFrame;
			
			if (IsLandscape (orientation))
			{
				orientationFrame.Size = new SizeF (orientationFrame.Size.Height, orientationFrame.Size.Width);
				statusBarFrame.Size = new SizeF (statusBarFrame.Size.Height, statusBarFrame.Size.Width);

			}
			
			float activeHeight = orientationFrame.Size.Height;
			
			if (keyboardHeight > 0)
				activeHeight += statusBarFrame.Size.Height * 2;
			
			activeHeight -= keyboardHeight;
			float posY = (float)Math.Floor (activeHeight * 0.45);
			float posX = orientationFrame.Size.Width / 2;

			if (!showToastCentered)
			{
				posY = activeHeight - 40;
			}
			
			PointF newCenter;
			float rotateAngle;

			switch (orientation)
			{ 
				case UIInterfaceOrientation.PortraitUpsideDown:
					rotateAngle = (float)Math.PI; 
					newCenter = new PointF (posX, orientationFrame.Size.Height - posY);
					break;
				case UIInterfaceOrientation.LandscapeLeft:
					rotateAngle = (float)(-Math.PI / 2.0f);
					newCenter = new PointF (posY, posX);
					break;
				case UIInterfaceOrientation.LandscapeRight:
					rotateAngle = (float)(Math.PI / 2.0f);
					newCenter = new PointF (orientationFrame.Size.Height - posY, posX);
					break;
				default: // as UIInterfaceOrientationPortrait
					rotateAngle = 0.0f;
					newCenter = new PointF (posX, posY);
					break;
			} 
			
			if (notification != null)
			{
				UIView.Animate (animationDuration,
				                0, UIViewAnimationOptions.AllowUserInteraction, delegate
				{
					MoveToPoint (newCenter, rotateAngle);
				}, null);

			} else
			{
				MoveToPoint (newCenter, rotateAngle);
			}
		}

		void SetFadeoutTimer (NSTimer newtimer)
		{
			if (_fadeoutTimer != null)
			{
				_fadeoutTimer.Invalidate ();
				_fadeoutTimer = null;
			}

			if (newtimer != null)
				_fadeoutTimer = newtimer;
		}

		void SetProgressTimer (NSTimer newtimer)
		{
			if (_progressTimer != null)
			{
				_progressTimer.Invalidate ();
				_progressTimer = null;
			}
		
			if (newtimer != null)
				_progressTimer = newtimer;
		}

		void UpdatePosition (bool textOnly = false)
		{
			float hudWidth = 100f;
			float hudHeight = 100f;
			float stringWidth = 0f;
			float stringHeight = 0f;
			RectangleF labelRect = new RectangleF ();
			
			string @string = StringLabel.Text;

			// False if it's text-only
			bool imageUsed = (ImageView.Image != null) || (ImageView.Hidden);
			if (textOnly)
				imageUsed = false;

			if (imageUsed)
				hudHeight = 80;
			else
				hudHeight = (textOnly ? 20 : 60);

			if (!string.IsNullOrEmpty (@string))
			{
				SizeF stringSize = new NSString (@string).StringSize (StringLabel.Font, new SizeF (200, 300));
				stringWidth = stringSize.Width;
				stringHeight = stringSize.Height;

				hudHeight += stringHeight;

				if (stringWidth > hudWidth)
					hudWidth = (float)Math.Ceiling (stringWidth / 2) * 2;
				
				float labelRectY = imageUsed ? 66 : 9;
				
				if (hudHeight > 100)
				{
					labelRect = new RectangleF (12, labelRectY, hudWidth, stringHeight);
					hudWidth += 24;
				} else
				{
					hudWidth += 24;
					labelRect = new RectangleF (0, labelRectY, hudWidth, stringHeight);
				}
			}

			// Adjust for Cancel Button
			var cancelRect = new RectangleF ();
			string @cancelCaption = CancelHudButton.Title (UIControlState.Normal);
			if (!string.IsNullOrEmpty (@cancelCaption))
			{
				const int gap = 20;
				SizeF stringSize = new NSString (@cancelCaption).StringSize (StringLabel.Font, new SizeF (200, 300));
				stringWidth = stringSize.Width;
				stringHeight = stringSize.Height;

				if (stringWidth > hudWidth)
					hudWidth = (float)Math.Ceiling (stringWidth / 2) * 2;

				// Adjust for label
				float cancelRectY = 0f;
				if (labelRect.Height > 0)
				{
					cancelRectY = labelRect.Y + labelRect.Height + gap;
				} else
				{
					cancelRectY = (imageUsed ? 66 : 9);
				}

				if (hudHeight > 100)
				{
					cancelRect = new RectangleF (12, cancelRectY, hudWidth, stringHeight);
					labelRect = new RectangleF (12, labelRect.Y, hudWidth, stringHeight);
					hudWidth += 24;
				} else
				{
					hudWidth += 24;
					cancelRect = new RectangleF (0, cancelRectY, hudWidth, stringHeight);
					labelRect = new RectangleF (0, labelRect.Y, hudWidth, stringHeight);
				}
				CancelHudButton.Frame = cancelRect;
				hudHeight += (cancelRect.Height + gap);
			}

			HudView.Bounds = new RectangleF (0, 0, hudWidth, hudHeight);
			if (!string.IsNullOrEmpty (@string))
				ImageView.Center = new PointF (HudView.Bounds.Width / 2, 36);
			else
				ImageView.Center = new PointF (HudView.Bounds.Width / 2, HudView.Bounds.Height / 2);


			StringLabel.Hidden = false;
			StringLabel.Frame = labelRect;

			if (!textOnly)
			{
				if (!string.IsNullOrEmpty (@string))
				{
					SpinnerView.Center = new PointF ((float)Math.Ceiling (HudView.Bounds.Width / 2.0f) + 0.5f, 40.5f);
					if (_progress != -1)
					{
						BackgroundRingLayer.Position = RingLayer.Position = new PointF (HudView.Bounds.Width / 2, 36f);
					}
				} else
				{
					SpinnerView.Center = new PointF ((float)Math.Ceiling (HudView.Bounds.Width / 2.0f) + 0.5f, (float)Math.Ceiling (HudView.Bounds.Height / 2.0f) + 0.5f);
					if (_progress != -1)
					{
						BackgroundRingLayer.Position = RingLayer.Position = new PointF (HudView.Bounds.Width / 2, HudView.Bounds.Height / 2.0f + 0.5f);
					}
				}
			}
		}

		public bool IsLandscape (UIInterfaceOrientation orientation)
		{
			return (orientation == UIInterfaceOrientation.LandscapeLeft || orientation == UIInterfaceOrientation.LandscapeRight);
		}

		public bool IsPortrait (UIInterfaceOrientation orientation)
		{
			return (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

