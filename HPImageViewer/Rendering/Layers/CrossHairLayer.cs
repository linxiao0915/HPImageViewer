
using HPImageViewer.Core.Primitives;
using HPImageViewer.Utils;
using System;
using System.Windows.Media;

namespace HPImageViewer.Rendering.Layers
{
    internal class CrossHairLayer : RenderBase, ILayer
    {
        static CrossHairLayer()
        {
            Pen = new Pen(Brushes.Gold, 0.8);
            Pen.Freeze();
        }


        private static readonly Pen Pen;
        protected override void OnRender(RenderContext renderContext)
        {
            var image = renderContext.Image;
            renderContext.RenderTransform.ToDevice(image.Width / 2, image.Height / 2, out var dx, out var dy);
            var width = renderContext.RenderSize.Width;
            var height = renderContext.RenderSize.Height;
            var drawingContext = renderContext.DrawingContext;
            if (Math.Abs(dx - width / 2) <= width / 2)
            {
                drawingContext.DrawLine(Pen, new Point(dx, 0), new Point(dx, height));
            }
            if (Math.Abs(dy - height / 2) <= height / 2)
            {
                drawingContext.DrawLine(Pen, new Point(0, dy), new Point(width, dy));
            }
        }

        protected override bool NeedRender(RenderContext renderContext)
        {
            var image = renderContext.Image;
            if (image == null) return false;

            renderContext.RenderTransform.ToDevice(image.Width / 2, image.Height / 2, out var dx, out var dy);
            var renderSize = renderContext.RenderSize;
            return Math.Abs(dx - renderSize.Width / 2) <= renderSize.Width / 2 || Math.Abs(dy - renderSize.Height / 2) <= renderSize.Height / 2;

        }
    }
}
