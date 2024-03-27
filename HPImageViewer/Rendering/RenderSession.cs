using System;
using System.Threading;
using System.Threading.Tasks;

namespace HPImageViewer.Rendering
{
    internal class RenderSession
    {
        IDrawingCanvas _drawingCanvas;
        public RenderContext RenderContext { get; }
        public RenderSession(IDrawingCanvas drawingCanvas, RenderContext renderContext)
        {
            _drawingCanvas = drawingCanvas;
            RenderContext = renderContext;

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
                        //  imageRender = new ImageRender(renderContext.Image) { RenderTransform = renderContext.RenderTransform };
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
            if (RenderContext.Image==null)
            {
                return null;
            }
            var imageRender = new ImageRender(RenderContext.Image) { RenderTransform = RenderContext.RenderTransform };
            imageRender.Calculate(RenderContext);
            return imageRender;
        }

    }
}
