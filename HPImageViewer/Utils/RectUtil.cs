using System;
using System.Windows;

namespace HPImageViewer.Utils
{
    internal class RectUtil
    {

        public static Rect GetNormalizedRectangle(double x1, double y1, double x2, double y2)
        {
            if (x2 < x1)
            {
                var tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1)
            {
                var tmp = y2;
                y2 = y1;
                y1 = tmp;
            }

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rect GetNormalizedRectangle(Point p1, Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Rect GetNormalizedRectangle(Rect r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        public static Rect GetOuterBoundRectangle(Rect r, double strokeThickness)
        {
            return new Rect(r.Left - strokeThickness / 2, r.Top - strokeThickness / 2, r.Width + strokeThickness, r.Height + strokeThickness);
        }
        public static Rect GetInnerBoundRectangle(Rect r, double strokeThickness)
        {
            var innerBoundWidth = r.Width - strokeThickness;
            var innerBoundHeight = r.Height - strokeThickness;
            var left = r.Left - strokeThickness / 2;
            var top = r.Top - strokeThickness / 2;

            if (innerBoundWidth <= 0)
            {
                innerBoundWidth = 0;
                left = (r.Left + r.Right) / 2;
            }
            if (innerBoundHeight <= 0)
            {
                innerBoundHeight = 0;
                top = (r.Top + r.Bottom) / 2;
            }

            return new Rect(left, top, innerBoundWidth, innerBoundHeight);
        }

        public static bool CircleIntersectsRectangle(double circleX, double circleY, double radius, Rect rect)
        {
            var rectX = rect.X;
            var rectY = rect.Y;
            var width = rect.Width;
            var height = rect.Height;
            // 计算圆心到矩形中心的距离
            // 计算圆心到矩形最近点的距离
            double closestX = Math.Max(rectX, Math.Min(circleX, rectX + width));
            double closestY = Math.Max(rectY, Math.Min(circleY, rectY + height));
            double distanceX = circleX - closestX;
            double distanceY = circleY - closestY;
            double distanceSquared = distanceX * distanceX + distanceY * distanceY;

            // 判断距离是否小于圆的半径
            return distanceSquared <= radius * radius;
        }

    }
}
