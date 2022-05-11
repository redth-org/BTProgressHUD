using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace BigTed
{
    public static class ShapeHelper
    {
        private static CGPoint PointOnCircle(float radius, float angleInDegrees)
        {
            float x = radius * MathF.Cos(angleInDegrees * MathF.PI / 180f) + radius;
            float y = radius * MathF.Sin(angleInDegrees * MathF.PI / 180f) + radius;
            return new CGPoint(x, y);
        }

        private static UIBezierPath CreateCirclePath(float radius, int sampleCount)
        {
            var smoothedPath = new UIBezierPath();
            CGPoint startPoint = PointOnCircle(radius, -90);

            smoothedPath.MoveTo(startPoint);

            float delta = 360f / sampleCount;
            float angleInDegrees = -90;
            for (int i = 1; i < sampleCount; i++)
            {
                angleInDegrees += delta;
                var point = PointOnCircle(radius, angleInDegrees);
                smoothedPath.AddLineTo(point);
            }
            smoothedPath.AddLineTo(startPoint);
            return smoothedPath;
        }

        public static CAShapeLayer CreateRingLayer(CGPoint center, float radius, float lineWidth, UIColor color)
        {
            var smoothedPath = CreateCirclePath(radius, 72);
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
