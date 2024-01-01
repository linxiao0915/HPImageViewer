using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering
{
    internal interface IDrawingContext
    {
        //todo:移除WPF框架元素,平台无关
        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle);
        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY);
        public void DrawLine(Pen pen, Point point0, Point point1);
        public void DrawPolygon(Brush brush, Pen pen, IEnumerable<Point> points);
        public void DrawText(FormattedText formattedText, Point point);
        public void DrawImage(ImageSource imageSource, Rect rectangle);


    }
}
