using System.Windows;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolPan : ITool, IMouseWheelTool
    {
        bool _startPanning = false;
        Point _startPint;
        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseDown(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            _startPanning = true;
            _startPint = e.GetPosition(drawingCanvas);
            drawingCanvas.CaptureMouse();

        }

        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {

            if (_startPanning == false) return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                _startPanning = false;
                return;
            }

            var currentPoint = e.GetPosition(drawingCanvas);

            drawingCanvas.PanTo(currentPoint.X - _startPint.X, currentPoint.Y - _startPint.Y);
            _startPint = currentPoint;
            drawingCanvas.InvalidateVisual();
        }

        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseUp(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            _startPanning = true;
            drawingCanvas.ReleaseMouseCapture();
        }

        public void OnMouseWheel(IDrawingCanvas drawingCanvas, MouseWheelEventArgs e)
        {
            ToolPointer.OnMouseWheelZoomingHandler(drawingCanvas, e);
        }
    }
}
