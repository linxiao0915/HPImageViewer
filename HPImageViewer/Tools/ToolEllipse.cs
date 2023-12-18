using HPImageViewer.Rendering.ROIRenders;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolEllipse : ToolRectangle
    {
        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            var center = e.GetPosition(drawingCanvas);

            var transformedCenter = drawingCanvas.CoordTransform.ToDomain(center);

            var rectangleRender = new EllipseRender() { CenterX = transformedCenter.X, CenterY = transformedCenter.Y, R = 1d / drawingCanvas.Scale };
            AddNewObject(drawingCanvas, rectangleRender);
            _isAdding = true;
        }



        //protected override void MoveHandle(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        //{
        //    var point = e.GetPosition(drawingCanvas);
        //    drawingCanvas.ROIRenders[0].MoveHandleTo(5, point);
        //    drawingCanvas.Rerender();
        //}

    }
}
