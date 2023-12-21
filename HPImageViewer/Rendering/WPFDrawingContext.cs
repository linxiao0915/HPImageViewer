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
