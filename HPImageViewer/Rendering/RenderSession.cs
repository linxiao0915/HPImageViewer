using System;
using System.Threading;
using System.Threading.Tasks;

namespace HPImageViewer.Rendering
{
    internal class RenderSession
    {


        IDrawingCanvas _drawingCanvas;

        public RenderSession(IDrawingCanvas drawingCanvas)
        {
            _drawingCanvas = drawingCanvas;

        }

        public async Task RenderAsync(CancellationToken cancellationToken, RenderContext renderContext, Action invalidateAction)
        {

            ImageRender imageRender = null;
            if (renderContext.Image != null)
            {
                await Task.Run(() =>
                {
                    imageRender = new ImageRender(renderContext.Image) { RenderTransform = renderContext.RenderTransform };
                    imageRender.Calculate(renderContext);

                }, cancellationToken);

            }

            if (cancellationToken.IsCancellationRequested == false)
            {
                _drawingCanvas.ImageRender = imageRender;
                invalidateAction();
            }

        }


    }
}
