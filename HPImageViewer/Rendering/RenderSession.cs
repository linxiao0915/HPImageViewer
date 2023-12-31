using System;
using System.Threading;
using System.Threading.Tasks;

namespace HPImageViewer.Rendering
{
    internal class RenderSession
    {
        IDrawingCanvas _drawingCanvas;
        RenderContext _renderContext;
        public RenderSession(IDrawingCanvas drawingCanvas, RenderContext renderContext)
        {
            _drawingCanvas = drawingCanvas;
            _renderContext = renderContext;

        }

        public async Task RenderDataAsync(CancellationToken cancellationToken, RenderContext renderContext, Func<CancellationToken, Task> invalidateAction, SemaphoreSlim semaphoreSlim)
        {
            try
            {
                await semaphoreSlim.WaitAsync(cancellationToken);
            }
            catch
            {
                return;
            }

            try
            {

                ImageRender imageRender = null;
                if (renderContext.Image != null)
                {
                    await Task.Run(() =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        imageRender = new ImageRender(renderContext.Image) { RenderTransform = renderContext.RenderTransform };
                        imageRender.Calculate(renderContext);

                    }, cancellationToken);

                }

                if (cancellationToken.IsCancellationRequested == false)
                {
                    _drawingCanvas.ImageRender = imageRender;
                    await invalidateAction(cancellationToken);

                }

            }
            finally
            {
                semaphoreSlim.Release();
            }


        }

        public ImageRender RenderData()
        {
            var imageRender = new ImageRender(_renderContext.Image) { RenderTransform = _renderContext.RenderTransform };
            imageRender.Calculate(_renderContext);
            return imageRender;
        }

    }
}
