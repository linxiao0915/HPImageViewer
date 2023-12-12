using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering
{
    internal interface IRenderContext
    {
        public ICoordTransform RenderTransform { get; }
        public Size RenderSize { get; }
        void DrawRectangle(Brush brush, Pen pen, Rect rectangle);
    }
}
