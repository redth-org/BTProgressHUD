using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace BTProgressHUD
{
    public static class ShapeHelper
    {
        public static CGPoint PointOnCircle(CGPoint center, float radius, float angleInDegrees)
        {
            float x = radius * (float)Math.Cos(angleInDegrees * Math.PI / 180) + radius;
            float y = radius * (float)Math.Sin(angleInDegrees * Math.PI / 180) + radius;
            return new CGPoint(x, y);
        }

        public static UIBezierPath CreateCirclePath(CGPoint center, float radius, int sampleCount)
        {
            var smoothedPath = new UIBezierPath();
            CGPoint startPoint = PointOnCircle(center, radius, -90);

            smoothedPath.MoveTo(startPoint);

            float delta = 360 / sampleCount;
            float angleInDegrees = -90;
            for (int i = 1; i < sampleCount; i++)
            {
                angleInDegrees += delta;
                var point = PointOnCircle(center, radius, angleInDegrees);
                smoothedPath.AddLineTo(point);
            }
            smoothedPath.AddLineTo(startPoint);
            return smoothedPath;
        }

        public static CAShapeLayer CreateRingLayer(CGPoint center, float radius, float lineWidth, UIColor color)
        {
            var smoothedPath = CreateCirclePath(center, radius, 72);
            var slice = new CAShapeLayer();
            slice.Frame = new CGRect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            slice.FillColor = UIColor.Clear.CGColor;
            slice.StrokeColor = color.CGColor;
            slice.LineWidth = lineWidth;
            slice.LineCap = CAShapeLayer.JoinBevel;
            slice.LineJoin = CAShapeLayer.JoinBevel;
            slice.Path = smoothedPath.CGPath;
            return slice;
        }
    }
}
