
using System.Windows.Media;
using HPImageViewer.Core.Primitives;

namespace HPImageViewer.Rendering.Layers
{
    internal class CrossHairLayer : RenderBase, IBackgroundLayer
    {
        static CrossHairLayer()
        {
            Pen = new Pen(Brushes.Gold, 0.8);
            Pen.Freeze();
        }


        private static readonly Pen Pen;
        protected override void OnRender(RenderContext renderContext)
        {
            var width = renderContext.RenderSize.Width;
            var height = renderContext.RenderSize.Height;
            if (width < 0 || height < 0)
            {
                return;
            }
            var drawingContext = renderContext.DrawingContext;
            drawingContext.DrawLine(Pen, new Point(0, height / 2), new Point(width, height / 2));
            drawingContext.DrawLine(Pen, new Point(width / 2, 0), new Point(width / 2, height));
        }
    }
}
