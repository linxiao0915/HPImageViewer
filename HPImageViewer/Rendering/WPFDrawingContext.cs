using HPImageViewer.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Point = HPImageViewer.Core.Primitives.Point;

namespace HPImageViewer.Rendering
{
    internal class WPFDrawingContext : IDrawingContext
    {
        private readonly DrawingContext _drawingContext;
        public WPFDrawingContext(DrawingContext drawingContext)
        {
            _drawingContext = drawingContext;
        }
        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle, double angle)
        {
            if (angle == 0)
            {
                _drawingContext.DrawRectangle(brush, pen, rectangle);
            }
            else
            {
                var rect = new RectangleGeometry() { Rect = rectangle, Transform = new RotateTransform(-angle, rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2) };

                _drawingContext.DrawGeometry(brush, pen, rect);
            }

        }

        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            _drawingContext.DrawEllipse(brush, pen, center.ToWindowPoint(), radiusX, radiusY);

        }
        public void DrawLine(Pen pen, Point point0, Point point1)
        {
            _drawingContext.DrawLine(pen, point0.ToWindowPoint(), point1.ToWindowPoint());
        }

        public void DrawPolygon(Brush brush, Pen pen, IEnumerable<Point> points)
        {
            if (points.Any() == false) return;

            var streamGeometry = new StreamGeometry();
            var pointList = points.ToList();

            using (var context = streamGeometry.Open())
            {
                var firstPoint = pointList.First();
                context.BeginFigure(firstPoint.ToWindowPoint(), true, true);
                pointList.RemoveAt(0);
                pointList.ForEach(n => context.LineTo(n.ToWindowPoint(), true, false));
            }
            _drawingContext.DrawGeometry(brush, pen, streamGeometry);
        }

        public void DrawText(FormattedText formattedText, Point point)
        {
            _drawingContext.DrawText(formattedText, point.ToWindowPoint());

        }
        public void DrawImage(ImageSource imageSource, Rect rectangle)
        {
            _drawingContext.DrawImage(imageSource, rectangle);
        }

    }
}
