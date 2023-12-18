using HPImageViewer.Rendering;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using OpenCvSharp;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace HPImageViewer
{
    internal interface IDrawingCanvas : IInputElement
    {
        public List<ROIRender> ROIRenders { get; set; }
        public ImageRender ImageRender { get; set; }
        public void InvalidateVisual(Rect? affectedArea = null);
        public Cursor Cursor { get; set; }
        public double Scale { get; }
        public Matrix TransformMatrix { get; }
        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY);
        public void PanTo(double deltaX, double deltaY);
        public void ResetView();
        public ICoordTransform CoordTransform { get; }
        public Size RenderSize { get; set; }
        public Mat Image { get; set; }

    }
}
