using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering
{
    internal interface IRenderContext
    {
        public ICoordTransform RenderTransform { get; }
        public Size RenderSize { get; }

        /// <summary>
        /// todo: 移除 DrawingContext，进行UI无关绘制
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="rectangle"></param>
        void DrawRectangle(Brush brush, Pen pen, Rect rectangle);
        /// <summary>
        ///todo: 移除 DrawingContext，进行UI无关绘制
        /// </summary>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="center"></param>
        /// <param name="radiusX"></param>
        /// <param name="radiusY"></param>
        void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY);
        /// <summary>
        ///todo: 移除 DrawingContext，进行UI无关绘制
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        void DrawLine(Pen pen, Point point0, Point point1);
    }
}
