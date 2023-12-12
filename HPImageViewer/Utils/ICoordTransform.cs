using System;
using System.Windows;

namespace HPImageViewer.Utils
{
    internal interface ICoordTransform
    {
        //void ToDomain(double dx, double dy, out double wx, out double wy);
        //void ToDevice(double wx, double wy, out double dx, out double dy);
        public Point ToDomain(Point devicePoint);
        public Point ToDevice(Point worldPoint);
        public Vector ToDomain(Vector deviceVector);
        public Vector ToDevice(Vector worldVector);

    }


    internal static class ICoordTransformExtensions
    {

        public static void ToDomain(this ICoordTransform coordTransform, double dx, double dy, out double wx, out double wy)
        {
            var worldPoint = coordTransform.ToDomain(new Point(dx, dy));
            wx = worldPoint.X;
            wy = worldPoint.Y;
        }

        public static void ToDevice(this ICoordTransform coordTransform, double wx, double wy, out double dx, out double dy)
        {
            var worldPoint = coordTransform.ToDevice(new Point(wx, wy));
            dx = worldPoint.X;
            dy = worldPoint.Y;
        }

        public static Rect ToDomain(this ICoordTransform coordTransform, Rect deviceRect)
        {
            return ToDeviceOrDomain(deviceRect, coordTransform.ToDomain);
        }
        public static Rect ToDevice(this ICoordTransform coordTransform, Rect worldRect)
        {
            return ToDeviceOrDomain(worldRect, coordTransform.ToDevice);
        }

        private static Rect ToDeviceOrDomain(Rect worldRect, Func<Point, Point> func)
        {
            var topLeft = worldRect.TopLeft;
            var transformedLeftTop = func(topLeft);
            var bottomRight = worldRect.BottomRight;
            var transformedBottomRight = func(bottomRight);
            return new Rect(transformedLeftTop, transformedBottomRight);
        }

    }
}
