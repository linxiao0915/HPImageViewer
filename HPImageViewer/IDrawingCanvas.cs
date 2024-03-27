using HPImageViewer.Rendering;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Size = System.Windows.Size;

namespace HPImageViewer
{
    internal interface IDrawingCanvas : IInputElement
    {
        ROIRenderCollection ROIRenderCollection { get; }
        ImageRender ImageRender { get; set; }
        void Rerender(bool immediate = true, RenderContext renderContext = null);
        Cursor Cursor { get; set; }
        double Scale { get; }
        Matrix TransformMatrix { get; }
        void ScaleAt(double scaleX, double scaleY, double centerX, double centerY);
        void PanTo(double deltaX, double deltaY);
        ICoordTransform CoordTransform { get; }
        Size RenderSize { get; set; }
        object Image { get; set; }
        bool FitNewImageToArea { get; set; }
        void FitImageToArea();



    }
}
