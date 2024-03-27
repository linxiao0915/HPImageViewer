using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal interface IMouseTool
    {
        void OnMouseWheel(IDrawingCanvas drawingCanvas, MouseWheelEventArgs e);
        void OnMouseDoubleClick(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e);

    }
}
