using System.Windows;

namespace HPImageViewer.Utils
{
    internal static class Extensions
    {
        public static Point ToWindowPoint(this Core.Primitives.Point point) => new Point(point.X, point.Y);
        public static Core.Primitives.Point ToPoint(this Point point) => new Core.Primitives.Point(point.X, point.Y);
        public static Rect ToWindowRect(this Core.Primitives.Rect rect) => new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        public static Core.Primitives.Rect ToRect(this Rect rect) => new Core.Primitives.Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
