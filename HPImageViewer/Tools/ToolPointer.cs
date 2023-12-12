﻿using HPImageViewer.Rendering.ROIRenders;
using System.Windows;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal class ToolPointer : ITool, IMouseWheelTool
    {
        private enum SelectionMode
        {
            None,
            NetSelection,   // group selection is active
            Move,           // object(s) are moves
            Size            // object is resized
        }
        private SelectionMode _selectMode = SelectionMode.None;

        private ROIRender resizedObject;
        private int resizedObjectHandle;
        // Keep state about last and current point (used to move and resize objects)
        private Point lastPoint = new Point(0, 0);
        private Point startPoint = new Point(0, 0);
        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseDown(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {

            _selectMode = SelectionMode.None;
            var point = e.GetPosition(drawingCanvas);

            foreach (var item in drawingCanvas.ROIRenders.GetSelectedItems())
            {
                int handleNumber = item.HitTest(point);

                if (handleNumber > 0)
                {
                    _selectMode = SelectionMode.Size;

                    // keep resized object in class member
                    resizedObject = item;
                    resizedObjectHandle = handleNumber;

                    // Since we want to resize only one object, unselect all other objects
                    drawingCanvas.ROIRenders.UnselectAll();
                    item.IsSelected = true;

                    //commandChangeState = new CommandChangeState(drawArea.GraphicsList);

                    break;
                }
            }

            if (_selectMode == SelectionMode.None)
            {
                int n1 = drawingCanvas.ROIRenders.Count;
                ROIRender o = null;

                for (int i = 0; i < n1; i++)
                {
                    if (drawingCanvas.ROIRenders[i].HitTest(point) == 0)
                    {
                        o = drawingCanvas.ROIRenders[i];
                        break;
                    }
                }

                if (o != null)
                {
                    _selectMode = SelectionMode.Move;
                    // Unselect all if Ctrl is not pressed and clicked object is not selected yet
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.LeftCtrl) == false && !o.IsSelected)
                    {
                        drawingCanvas.ROIRenders.UnselectAll();
                    }

                    o.IsSelected = true;


                    // commandChangeState = new CommandChangeState(drawArea.GraphicsList);

                    drawingCanvas.Cursor = Cursors.SizeAll;
                }
            }
            // Net selection
            if (_selectMode == SelectionMode.None)
            {
                // click on background
                if (Keyboard.IsKeyDown(Key.LeftCtrl) == false && Keyboard.IsKeyDown(Key.LeftCtrl) == false)
                    drawingCanvas.ROIRenders.UnselectAll();

                _selectMode = SelectionMode.NetSelection;

            }


            lastPoint.X = point.X;
            lastPoint.Y = point.Y;
            drawingCanvas.CaptureMouse();
            drawingCanvas.InvalidateVisual();

        }

        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            var point = e.GetPosition(drawingCanvas);
            var oldPoint = lastPoint;


            // set cursor when mouse button is not pressed
            if (e.LeftButton == MouseButtonState.Released &&
                e.MiddleButton == MouseButtonState.Released &&
                e.RightButton == MouseButtonState.Released)
            {
                Cursor cursor = null;

                for (int i = 0; i < drawingCanvas.ROIRenders.Count; i++)
                {
                    int n = drawingCanvas.ROIRenders[i].HitTest(point);

                    if (n > 0)
                    {
                        //  cursor = drawingCanvas.ROIRenders[i].GetHandleCursor(n);
                        break;
                    }
                }

                if (cursor == null)
                    cursor = Cursors.Arrow;

                drawingCanvas.Cursor = cursor;

                return;
            }

            if (e.LeftButton == MouseButtonState.Released)
                return;

            /// Left button is pressed

            // Find difference between previous and current position
            var dx = point.X - lastPoint.X;
            var dy = point.Y - lastPoint.Y;

            lastPoint.X = point.X;
            lastPoint.Y = point.Y;

            // resize
            if (_selectMode == SelectionMode.Size)
            {
                if (resizedObject != null)
                {
                    resizedObject.MoveHandleTo(resizedObjectHandle, point);
                    // drawingCanvas.SetDirty();
                    drawingCanvas.InvalidateVisual();
                }
            }

            // move
            if (_selectMode == SelectionMode.Move)
            {
                foreach (var o in drawingCanvas.ROIRenders.GetSelectedItems())
                {
                    o.MoveTo(dx, dy);
                }

                drawingCanvas.Cursor = Cursors.SizeAll;
                // drawArea.SetDirty();
                drawingCanvas.InvalidateVisual();
            }

            //if (selectMode == SelectionMode.NetSelection)
            //{
            //    // Remove old selection rectangle
            //    ControlPaint.DrawReversibleFrame(
            //        drawArea.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, oldPoint)),
            //        Color.Black,
            //        FrameStyle.Dashed);

            //    // Draw new selection rectangle
            //    ControlPaint.DrawReversibleFrame(
            //        drawArea.RectangleToScreen(DrawRectangle.GetNormalizedRectangle(startPoint, point)),
            //        Color.Black,
            //        FrameStyle.Dashed);

            //    return;
            //}
        }

        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public void OnMouseUp(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {


            if (resizedObject != null)
            {
                // after resizing
                resizedObject.Normalize();
                resizedObject = null;
            }



            //if ( commandChangeState != null  && wasMove )
            //{
            //    // Keep state after moving/resizing and add command to history
            //    commandChangeState.NewState(drawArea.GraphicsList);
            //    drawArea.AddCommandToHistory(commandChangeState);
            //    commandChangeState = null;
            //}

            this._selectMode = SelectionMode.None;
            drawingCanvas.ReleaseMouseCapture();
            drawingCanvas.InvalidateVisual();
        }



        public void OnMouseWheel(IDrawingCanvas drawingCanvas, MouseWheelEventArgs e)
        {
            if (_selectMode != SelectionMode.None) return;
            // set cursor when mouse button is not pressed
            if (e.LeftButton == MouseButtonState.Pressed
                || e.MiddleButton == MouseButtonState.Pressed
                || e.RightButton == MouseButtonState.Pressed) return;
            OnMouseWheelZoomingHandler(drawingCanvas, e);

        }

        public static void OnMouseWheelZoomingHandler(IDrawingCanvas drawingCanvas, MouseWheelEventArgs e)
        {

            var point = e.GetPosition(drawingCanvas);

            double radio = 1;
            if (e.Delta > 0)
            {
                radio = 1.1;//(scale + 0.05) / scale;
                drawingCanvas.Scale *= 1.1;
            }
            else
            {
                radio = 0.95;
                drawingCanvas.Scale *= 0.95;
            }

            drawingCanvas.ScaleAt(radio, radio, point.X, point.Y);
            drawingCanvas.InvalidateVisual();
        }
    }
}