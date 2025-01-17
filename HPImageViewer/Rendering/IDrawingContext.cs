﻿using System.Collections.Generic;
using System.Windows.Media;
using Point = HPImageViewer.Core.Primitives.Point;

namespace HPImageViewer.Rendering
{
    public interface IDrawingContext
    {
        //todo:移除WPF框架元素,平台无关
        public void DrawRectangle(Brush brush, Pen pen, HPImageViewer.Core.Primitives.Rect rectangle, double angle);
        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY);
        public void DrawLine(Pen pen, Point point0, Point point1);
        public void DrawPolygon(Brush brush, Pen pen, IEnumerable<Point> points);
        public void DrawText(FormattedText formattedText, Point point);
        public void DrawImage(ImageSource imageSource, HPImageViewer.Core.Primitives.Rect rectangle);
    }

    public static class DrawingContextExtensions
    {
        public static void DrawRegularRectangle(this IDrawingContext drawingContext, Brush brush, Pen pen, HPImageViewer.Core.Primitives.Rect rectangle)
        {
            drawingContext.DrawRectangle(brush, pen, rectangle, 0);
        }
    }
}
