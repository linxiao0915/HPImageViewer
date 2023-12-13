using HPImageViewer.Rendering.ROIRenders;
using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal abstract class ToolObject : ITool
    {
        protected bool _isAdding = false;
        private Cursor _Cursor;
        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseDown(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            if (_isAdding)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    _isAdding = false;
                    return;
                }
                MoveHandle(drawingCanvas, e);
            }
        }
        protected virtual void MoveHandle(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {

        }


        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public virtual void OnMouseUp(IDrawingCanvas drawingCanvas, MouseEventArgs e)
        {
            _isAdding = false;
        }


        public static void AddNewObject(IDrawingCanvas drawingCanvas, ROIRender roiRender)
        {
            roiRender.IsSelected = true;
            roiRender.RenderTransform = drawingCanvas.CoordTransform;
            drawingCanvas.ROIRenders.Insert(0, roiRender);
        }

    }
}
