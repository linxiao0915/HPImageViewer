using HPImageViewer.Utils;
using System.Windows;

namespace HPImageViewer.Rendering
{
    internal interface IRenderContext
    {
        public ICoordTransform RenderTransform { get; }
        public Size RenderSize { get; }


    }
}
