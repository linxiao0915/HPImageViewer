﻿using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolRectangle : ToolObject
    {

        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            var leftTop = e.GetPosition(drawingCanvas);
            var transformedLeftTop = drawingCanvas.CoordTransform.ToDomain(leftTop.ToPoint());
            var rectangleRender = new RectangleRender() { Left = transformedLeftTop.X, Top = transformedLeftTop.Y, Width = 1d / drawingCanvas.Scale, Height = 1d / drawingCanvas.Scale };
            PrepareNewObject(drawingCanvas, rectangleRender);

            _isAdding = true;
        }


        protected override void MoveHandle(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            var point = e.GetPosition(drawingCanvas);
            drawingCanvas.ROIRenderCollection.AddingRoiRender?.MoveHandleTo(5, point);

            drawingCanvas.Rerender();
        }

    }
}
