using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace HPImageViewer.Utils
{
    internal static class Extensions
    {
        public static Point ToWindowPoint(this Core.Primitives.Point point) => new Point(point.X, point.Y);
        public static Core.Primitives.Point ToPoint(this Point point) => new Core.Primitives.Point(point.X, point.Y);
    }
}
