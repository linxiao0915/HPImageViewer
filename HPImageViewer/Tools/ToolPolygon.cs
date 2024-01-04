﻿using HPImageViewer.Rendering.ROIRenders;
using System.Windows;
using System.Windows.Input;
using HPImageViewer.Utils;

namespace HPImageViewer.Tools
{
    internal class ToolPolygon : ToolObject
    {


        PolygonRender _polygonRender;
        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseDown(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {


        }

        private Point? _floatPoint;
        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(drawingCanvas);
            var transformedStartPoint = drawingCanvas.CoordTransform.ToDomain(currentPoint);
            if (_isAdding)
            {
                var polygonRender = ((PolygonRender)drawingCanvas.ROIRenders[0]);
                if (_floatPoint.HasValue)
                {
                    polygonRender.Points.RemoveAt(polygonRender.Points.Count - 1);
                }
                polygonRender.Points.Add(transformedStartPoint.ToPoint());
                _floatPoint = transformedStartPoint;
                drawingCanvas.Rerender();
            }
        }

        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(IDrawingCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            // base.OnMouseUp(drawingCanvas, e);

            if (_floatPoint != null)
            {
                _floatPoint = null;
                _polygonRender.Points.RemoveAt(_polygonRender.Points.Count - 1);
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                var startPoint = e.GetPosition(drawingCanvas);
                var transformedStartPoint = drawingCanvas.CoordTransform.ToDomain(startPoint);

                if (_polygonRender == null)
                {
                    _polygonRender = new PolygonRender() { IsClosed = false };
                    AddNewObject(drawingCanvas, _polygonRender);
                }



                _polygonRender.Points.Add(transformedStartPoint.ToPoint());
                _isAdding = true;
                drawingCanvas.Rerender();
                return;
            }

            if (_isAdding)
            {
                if (_polygonRender.Points.Count >= 3)
                {
                    _polygonRender.IsClosed = true;
                }
                else
                {
                    drawingCanvas.ROIRenders.Remove(_polygonRender);
                }
                e.Handled = true;
                _isAdding = false;
                _polygonRender = null;
                drawingCanvas.Rerender();

            }
        }




    }
}
