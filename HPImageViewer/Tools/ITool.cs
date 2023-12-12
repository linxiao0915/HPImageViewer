using System.Windows.Input;

namespace HPImageViewer.Tools
{
    internal interface ITool
    {

        /// <summary>
        /// Left mouse button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        void OnMouseDown(IDrawingCanvas drawingCanvas, MouseEventArgs e);


        /// <summary>
        /// Mouse is moved, left mouse button is pressed or none button is pressed
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        void OnMouseMove(IDrawingCanvas drawingCanvas, MouseEventArgs e);


        /// <summary>
        /// Left mouse button is released
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        void OnMouseUp(IDrawingCanvas drawingCanvas, MouseEventArgs e);

    }
}
