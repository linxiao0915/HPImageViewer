using System;
using System.Windows;

namespace HPImageViewer.Utils
{
    public interface ICoordTransform
    {
        //void ToDomain(double dx, double dy, out double wx, out double wy);
        //void ToDevice(double wx, double wy, out double dx, out double dy);
        public HPImageViewer.Core.Primitives.Point ToDomain(HPImageViewer.Core.Primitives.Point devicePoint);
        public HPImageViewer.Core.Primitives.Point ToDevice(HPImageViewer.Core.Primitives.Point worldPoint);
        public Vector ToDomain(Vector deviceVector);
        public Vector ToDevice(Vector worldVector);

    }


    internal static class ICoordTransformExtensions
    {

        public static void ToDomain(this ICoordTransform coordTransform, double dx, double dy, out double wx, out double wy)
        {
            var worldPoint = coordTransform.ToDomain(new HPImageViewer.Core.Primitives.Point(dx, dy));
            wx = worldPoint.X;
            wy = worldPoint.Y;
        }

        public static void ToDevice(this ICoordTransform coordTransform, double wx, double wy, out double dx, out double dy)
        {
            var worldPoint = coordTransform.ToDevice(new HPImageViewer.Core.Primitives.Point(wx, wy));
            dx = worldPoint.X;
            dy = worldPoint.Y;
        }

        public static HPImageViewer.Core.Primitives.Rect ToDomain(this ICoordTransform coordTransform, HPImageViewer.Core.Primitives.Rect deviceRect)
        {
            return ToDeviceOrDomain(deviceRect, coordTransform.ToDomain);
        }
        public static HPImageViewer.Core.Primitives.Rect ToDevice(this ICoordTransform coordTransform, HPImageViewer.Core.Primitives.Rect worldRect)
        {
            return ToDeviceOrDomain(worldRect, coordTransform.ToDevice);
        }

        private static HPImageViewer.Core.Primitives.Rect ToDeviceOrDomain(HPImageViewer.Core.Primitives.Rect worldRect, Func<HPImageViewer.Core.Primitives.Point, HPImageViewer.Core.Primitives.Point> func)
        {
            var topLeft = worldRect.TopLeft;
            var transformedLeftTop = func(topLeft);
            var bottomRight = worldRect.BottomRight;
            var transformedBottomRight = func(bottomRight);
            return new HPImageViewer.Core.Primitives.Rect(transformedLeftTop, transformedBottomRight);
        }

    }
}
