using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal interface IMouseWheelTool
    {
        void OnMouseWheel(IDrawingCanvas drawingCanvas, MouseWheelEventArgs e);
    }
}
