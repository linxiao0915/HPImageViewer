using HPImageViewer.Core;
using HPImageViewer.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Size = HPImageViewer.Core.Primitives.Size;


namespace HPImageViewer.Rendering
{
    public class ImageRender : RenderBase, IDisposable
    {

        // ImageSource _imageSource;
        object _image = null;
        public ImageRender(object image)
        {
            _image = image;
        }

        public Size ImageSize
        {
            get; private set;
        }
        private HPImageViewer.Core.Primitives.Rect ImageDeviceRect => this.RenderTransform.ToDevice(new HPImageViewer.Core.Primitives.Rect(0, 0, ImageSize.Width, ImageSize.Height));

        protected override bool NeedRender(RenderContext renderContext)
        {
            if (ImageSize.Width <= 0 || ImageSize.Height <= 0 || _renderImage == null) return false;
            if (ImageDeviceRect.IntersectsWith(new HPImageViewer.Core.Primitives.Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height)) == false) return false;

            //是否落到区域内
            return base.NeedRender(renderContext);
        }

        protected override void OnRender(RenderContext renderContext)
        {

            //var deviceDrawingArea = new Rect(-renderContext.RenderSize.Width / 2, -renderContext.RenderSize.Height / 2, renderContext.RenderSize.Width * 2, renderContext.RenderSize.Height * 2);//2倍区域绘制,拖拽时的缓存效果
            //deviceDrawingArea.Intersect(ImageDeviceRect);
#if DEBUG
            // 创建 Stopwatch 实例
            Stopwatch stopwatch = new Stopwatch();
#endif


#if DEBUG
            // 开始计时
            stopwatch.Restart();
#endif
            renderContext.DrawingContext.DrawImage(_renderImage, renderContext.RenderTransform.ToDevice(_renderImageRect));

#if DEBUG
            stopwatch.Stop();
            //Console.WriteLine($"DrawingContextDrawing 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif
        }

        HPImageViewer.Core.Primitives.Rect GetRoundedImagePixelArea(double imageWidth, double imageHeight, HPImageViewer.Core.Primitives.Rect renderImagePixelArea)
        {
            var left = Math.Floor(renderImagePixelArea.Left);
            if (left < 0)
            {
                left = 0;
            }
            var top = Math.Floor(renderImagePixelArea.Top);
            if (top < 0)
            {
                top = 0;
            }

            var right = Math.Ceiling(renderImagePixelArea.Right);
            if (right >= imageWidth)
            {
                right = imageWidth;
            }

            var bottom = Math.Ceiling(renderImagePixelArea.Bottom);
            if (bottom >= imageHeight)
            {
                bottom = imageHeight;
            }

            return new HPImageViewer.Core.Primitives.Rect(left, top, right - left, bottom - top);
        }

        public void Calculate(RenderContext renderContext)
        {
            _renderImage = null;
#if DEBUG
            var stopwatch = new Stopwatch();
#endif

            //  var deviceDrawingArea = new Rect(0, 0, renderContext.RenderSize.Width , renderContext.RenderSize.Height);
            //var expandedWidth = renderContext.RenderSize.Width * 0.1;//扩展区域绘制,拖拽时的缓存效果
            //var expandedHeight = renderContext.RenderSize.Height * 0.1;
            var deviceDrawingArea = new HPImageViewer.Core.Primitives.Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height);//2倍区域绘制,拖拽时的缓存效果
            using var indexer = AggregationIndexerFactory.Instance.CreatePixelDataIndexer(_image);
            ImageSize = indexer.ImageSize;
            deviceDrawingArea.Intersect(ImageDeviceRect);
            if (deviceDrawingArea.IsEmpty)
            {//数值范围溢出
                return;
            }

            //绘制到设备的区域
            var renderDeviceArea = deviceDrawingArea; //绘制的Image像素区域
            var renderImagePixelArea = RenderTransform.ToDomain(renderDeviceArea);


            //绘制的Image像素区域
            var roundedImagePixelRect = GetRoundedImagePixelArea(ImageSize.Width, ImageSize.Height, renderImagePixelArea);

            var finalRoundedImagePixelRect = new HPImageViewer.Core.Primitives.Rect(roundedImagePixelRect.X, roundedImagePixelRect.Y, roundedImagePixelRect.Width, roundedImagePixelRect.Height);
            //剪裁图片
            //   using var clippedMat = new Mat(_mat, roundedImagePixelRect);
            //HOperatorSet.CropPart(_mat, out var clippedImage, new HTuple(roundedImagePixelRect.Top), new HTuple(roundedImagePixelRect.Left), new HTuple(roundedImagePixelRect.Width), new HTuple(roundedImagePixelRect.Height));
            //var clippedMat = clippedImage;
            var deviceDrawingWidth = deviceDrawingArea.Width < 1 ? 1 : deviceDrawingArea.Width;
            var deviceDrawingHeight = deviceDrawingArea.Height < 1 ? 1 : deviceDrawingArea.Height;




#if DEBUG
            // 开始计时
            stopwatch.Restart();
#endif

            _renderImage = ToWriteableBitmap(indexer, finalRoundedImagePixelRect, new Size(deviceDrawingWidth, deviceDrawingHeight));
            _renderImage.Freeze();
            _renderImageRect = finalRoundedImagePixelRect;
#if DEBUG
            stopwatch.Stop();
            //Console.WriteLine($"ToWriteableBitmap 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif

#if DEBUG
            // 开始计时
            //stopwatch.Restart();
#endif

#if DEBUG
            //stopwatch.Stop();
            //Console.WriteLine($"DrawingContextDrawing 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif
        }


        private ImageSource _renderImage;
        private HPImageViewer.Core.Primitives.Rect _renderImageRect;
        //   private Matrix _transformMatrix;
        //private ICoordTransform _coordTransformCache;



        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {

        }

        public static unsafe WriteableBitmap ToWriteableBitmap(PixelDataIndexer indexer, Core.Primitives.Rect clipImageRect, Core.Primitives.Size deviceMapSize)
        {
            var watch = Stopwatch.StartNew();


            //获取通道数

            try
            {
                var pixelFormats = indexer.ChannelCount == 1 ? PixelFormats.Gray8 : PixelFormats.Rgb24;
                var resizeHeight = (int)Math.Ceiling(deviceMapSize.Height);
                var resizeWidth = (int)Math.Ceiling(deviceMapSize.Width);
                var scaleX = clipImageRect.Width / resizeWidth;
                var scaleY = clipImageRect.Height / resizeHeight;

                var dest = new WriteableBitmap(resizeWidth, resizeHeight, 96, 96, pixelFormats, null);

                dest.Lock();
                var stride = dest.BackBufferStride;

                //var srcPointers = hPointers.Select(s => s.IP).ToList();
                var byteCountPerPixel = indexer.ChannelCount;
                var desPointer = (byte*)dest.BackBuffer.ToPointer();

                var startCol = clipImageRect.Left;
                var startRow = clipImageRect.Top;
                indexer.LockData();
                Parallel.For(0, resizeHeight, row =>
                {
                    //先转到剪裁系下
                    var rowInClip = Math.Floor(scaleY * row);
                    var rowInSrc = startRow + rowInClip;
                    for (var i = 0; i < resizeWidth; i++)
                    {
                        var colInClip = Math.Floor(scaleX * i);
                        var colInSrc = startCol + colInClip;

                        for (var j = 0; j < byteCountPerPixel; j++)
                        {
                            var offsetInDes = (long)row * (long)stride + (long)i * (long)byteCountPerPixel + (long)j;
                            *(desPointer + offsetInDes) = indexer.GetPixelData(j, (int)rowInSrc, (int)colInSrc);
                        }
                    }
                });
                indexer.UnlockData();
                dest.AddDirtyRect(new Int32Rect(0, 0, dest.PixelWidth, dest.PixelHeight));
                dest.Unlock();

                return dest;
            }
            //catch (Exception e)
            //{
            //    //  isSuccess = false;
            //    // Logger?.LogDebug(e, $"将 HObject 转换为 {pixelFormat} Bitmap 时发生错误：{e.Message}");
            //    throw;
            //}
            finally
            {

                watch.Stop();
                //if (Debugger.IsAttached)
                //{
                //    Logger?.LogInformation($"将 HObject 转换为 {pixelFormat} Bitmap {(isSuccess ? "成功" : "失败")}! 耗时:{watch.ElapsedMilliseconds} ms");
                //}
            }
            //return null;
        }
    }
}
