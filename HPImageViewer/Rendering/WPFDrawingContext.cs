using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering
{
    internal class WPFDrawingContext : IDrawingContext
    {
        private readonly DrawingContext _drawingContext;
        public WPFDrawingContext(DrawingContext drawingContext)
        {
            _drawingContext = drawingContext;
        }
        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            _drawingContext.DrawRectangle(brush, pen, rectangle);

        }

        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            _drawingContext.DrawEllipse(brush, pen, center, radiusX, radiusY);

        }
        public void DrawLine(Pen pen, Point point0, Point point1)
        {
            _drawingContext.DrawLine(pen, point0, point1);
            //_drawingContext.DrawGeometry();
        }

        public void DrawPolygon(Brush brush, Pen pen, IEnumerable<Point> points)
        {
            if (points.Any() == false) return;

            var streamGeometry = new StreamGeometry();
            var pointList = points.ToList();

            using (var context = streamGeometry.Open())
            {
                var firstPoint = pointList.First();
                context.BeginFigure(firstPoint, true, true);
                pointList.RemoveAt(0);
                pointList.ForEach(n => context.LineTo(n, true, false));
            }
            _drawingContext.DrawGeometry(brush, pen, streamGeometry);
        }

        public void DrawText(FormattedText formattedText, Point point)
        {
            _drawingContext.DrawText(formattedText, point);

        }
        public void DrawImage(ImageSource imageSource, Rect rectangle)
        {
            _drawingContext.DrawImage(imageSource, rectangle);
        }

    }
}
