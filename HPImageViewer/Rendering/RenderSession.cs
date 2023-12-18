using System;
using System.Diagnostics;
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

        public async Task RenderAsync(CancellationToken cancellationToken, RenderContext renderContext, Action invalidateAction, SemaphoreSlim semaphoreSlim)
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
                {            //deviceDrawingArea.Intersect(ImageDeviceRect);

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
                    invalidateAction();

                }


            }
            finally
            {
                semaphoreSlim.Release();
            }


        }


    }
}
