using HPImageViewer.Core;
using HPImageViewer.Core.Primitives;
using HPImageViewer.Utils;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;

namespace HPImageViewer.Behaviors
{
    internal class PixelIndicatorBehavior : Behavior<ImageViewer>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            var x = 0;
            var y = 0;
            var grayScaleValue = double.NaN;
            var renderSize = AssociatedObject.ImageViewDrawCanvas.RenderSize;
            var mousePos = Mouse.GetPosition(AssociatedObject.ImageViewDrawCanvas);
            var rect = new Rect(0, 0, renderSize.Width, renderSize.Height);
            var pixelInfoText = "";
            if (rect.Contains(mousePos.ToPoint()) && AssociatedObject.ImageViewDrawCanvas.Image != null)
            {
                var worldPoint = AssociatedObject.ImageViewDrawCanvas.CoordTransform.ToDomain(mousePos.ToPoint());
                x = (int)worldPoint.X;
                y = (int)worldPoint.Y;
                using var indexer = AggregationIndexerFactory.Instance.CreatePixelDataIndexer(AssociatedObject.ImageViewDrawCanvas.Image);
                var imageRect = new Rect(0, 0, indexer.ImageSize.Width, indexer.ImageSize.Height);

                if (worldPoint.X < indexer.ImageSize.Width
                    && worldPoint.X >= 0
                    && worldPoint.Y < indexer.ImageSize.Height
                    && worldPoint.Y >= 0)
                {
                    if (indexer.ChannelCount == 1)
                    {
                        grayScaleValue = indexer.GetPixelData(0, y, x);
                        pixelInfoText = $"[Gray: {grayScaleValue}]";
                    }
                    else if (indexer.ChannelCount == 3)
                    {   //0.2989×R+0.5870×G+0.1140×B
                        grayScaleValue = 0.2989 * indexer.GetPixelData(0, y, x) + 0.5870 * indexer.GetPixelData(1, y, x) + 0.1140 * indexer.GetPixelData(2, y, x);
                        pixelInfoText = $"[R: {indexer.GetPixelData(0, y, x)}, G: {indexer.GetPixelData(1, y, x)}, B:{indexer.GetPixelData(1, y, x)}]";
                    }
                }

            }

            AssociatedObject.PixelIndicator.Text = $"[Scale: {(AssociatedObject.ImageViewDrawCanvas.Scale * 100):F2}%]"
                + $" [X: {x}; Y: {y}]"
                + $" {pixelInfoText}";
        }


        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            base.OnDetaching();
        }
    }
}
