using HPImageViewer.Core;
using HPImageViewer.Rendering;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.RoutedEventArgs;
using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolPan : ITool, IMouseTool
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
            drawingCanvas.Rerender();
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

        public void OnMouseDoubleClick(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            var renderSize = drawingCanvas.RenderSize;
            var mousePos = Mouse.GetPosition(drawingCanvas);
            var rect = new HPImageViewer.Core.Primitives.Rect(0, 0, renderSize.Width, renderSize.Height);
            var image = drawingCanvas.Image;
            if (rect.Contains(mousePos.ToPoint()) == false || image == null) return;
            var worldPoint = drawingCanvas.CoordTransform.ToDomain(mousePos.ToPoint());
            var x = (int)worldPoint.X;
            var y = (int)worldPoint.Y;
            using var indexer = AggregationIndexerFactory.Instance.CreatePixelDataIndexer(drawingCanvas.Image);
            //    var imageRect = new Rect(0, 0, indexer.ImageSize.Width, indexer.ImageSize.Height);
            if (x < indexer.ImageSize.Width
                  && x >= 0
                  && y < indexer.ImageSize.Height
                  && y >= 0)
            {
                var args = new ImageDoubleClickedRoutedEventArgs(ImageView.ImageDoubleClickedEvent)
                {
                    ImageSize = indexer.ImageSize,
                    Position = new Point(x, y)
                };

                drawingCanvas.RaiseEvent(args);
            }
            //发起图片被双击事件



        }
    }
}
