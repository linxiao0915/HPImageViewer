using HPImageViewer.Rendering.ROIRenders;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolRectangle : ToolObject
    {
        private bool _isAdding = false;

        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            base.OnMouseDown(drawingCanvas, e);
            var leftTop = e.GetPosition(drawingCanvas);
            var transformedLeftTop = drawingCanvas.CoordTransform.ToDomain(leftTop);
            var rectangleRender = new RectangleRender() { Left = transformedLeftTop.X, Top = transformedLeftTop.Y, Width = 1d, Height = 1d, RenderTransform = drawingCanvas.CoordTransform };
            drawingCanvas.ROIRenders.Insert(0, rectangleRender);
            _isAdding = true;
        }

        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            base.OnMouseMove(drawingCanvas, e);
            if (_isAdding)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    _isAdding = false;
                }
                var point = e.GetPosition(drawingCanvas);
                drawingCanvas.ROIRenders[0].MoveHandleTo(5, point);
                drawingCanvas.InvalidateVisual();
            }
        }

        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            _isAdding = false;
            base.OnMouseUp(drawingCanvas, e);
        }
    }
}
