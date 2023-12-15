using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering
{
    internal class RenderContext : IRenderContext
    {

        public RenderContext(DrawingContext drawingContext)
        {
            DrawingContext = drawingContext;
        }

        public List<ROIDesc> RoiDescs
        {
            get;
            set;
        }

        public OpenCvSharp.Mat Image { get; set; }

        //todo: 移除 DrawingContext，进行UI无关绘制
        public DrawingContext DrawingContext { get; private set; }

        public void DrawRectangle(Brush brush, Pen pen, Rect rectangle)
        {
            DrawingContext.DrawRectangle(brush, pen, rectangle);

        }

        public void DrawEllipse(Brush brush, Pen pen, Point center, double radiusX, double radiusY)
        {
            DrawingContext.DrawEllipse(brush, pen, center, radiusX, radiusY);

        }
        public void DrawLine(Pen pen, Point point0, Point point1)
        {
            DrawingContext.DrawLine(pen, point0, point1);
        }





        public double Scale { get; set; } = 1;
        public Matrix TransformMatrix { get; set; }
        private RenderTransform _RenderTransform;
        public ICoordTransform RenderTransform
        {
            get

            {
                _RenderTransform ??= new RenderTransform(TransformMatrix);
                return _RenderTransform;
            }
        }

        public Size RenderSize { get; set; }


        //todo:transformer 构造，管理处理变换。
        //todo:元素Merge 批量绘制性能优化,不在区域不进行绘制
        //todo:多实现 D3D OpenGL WPF Winform
    }
}
