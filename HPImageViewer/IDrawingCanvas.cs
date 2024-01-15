using HPImageViewer.Rendering;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using OpenCvSharp;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Rect = System.Windows.Rect;
using Size = System.Windows.Size;

namespace HPImageViewer
{
    internal interface IDrawingCanvas : IInputElement
    {
        ROIRenderCollection ROIRenderCollection { get; }
        ImageRender ImageRender { get; set; }
        void Rerender(Rect? affectedArea = null, bool immediate = true);
        Cursor Cursor { get; set; }
        double Scale { get; }
        Matrix TransformMatrix { get; }
        void ScaleAt(double scaleX, double scaleY, double centerX, double centerY);
        void PanTo(double deltaX, double deltaY);
        ICoordTransform CoordTransform { get; }
        Size RenderSize { get; set; }
        Mat Image { get; set; }
        bool FitNewImageToArea { get; set; }
        void FitImageToArea();



    }
}
