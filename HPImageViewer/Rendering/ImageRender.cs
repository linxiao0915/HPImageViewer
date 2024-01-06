using HPImageViewer.Utils;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Diagnostics;
using System.Windows.Media;
using Rect = System.Windows.Rect;

namespace HPImageViewer.Rendering
{
    internal class ImageRender : RenderBase, IDisposable
    {

        // ImageSource _imageSource;
        Mat _mat = new Mat(0, 0, MatType.CV_8U);
        public ImageRender(Mat mat)
        {
            _mat = mat;
        }

        private Rect ImageDeviceRect => this.RenderTransform.ToDevice(new Rect(0, 0, _mat.Width, _mat.Height));

        protected override bool NeedRender(RenderContext renderContext)
        {
            if (_mat.Width <= 0 || _mat.Height <= 0 || _renderImage == null) return false;
            if (ImageDeviceRect.IntersectsWith(new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height)) == false) return false;

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
            Console.WriteLine($"DrawingContextDrawing 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif
        }

        OpenCvSharp.Rect GetRoundedImagePixelArea(int imageWidth, int imageHeight, Rect renderImagePixelArea)
        {
            var left = (int)Math.Floor(renderImagePixelArea.Left);
            if (left < 0)
            {
                left = 0;
            }
            var top = (int)Math.Floor(renderImagePixelArea.Top);
            if (top < 0)
            {
                top = 0;
            }

            var right = (int)Math.Ceiling(renderImagePixelArea.Right);
            if (right >= imageWidth)
            {
                right = imageWidth;
            }

            var bottom = (int)Math.Ceiling(renderImagePixelArea.Bottom);
            if (bottom >= imageHeight)
            {
                bottom = imageHeight;
            }

            return new OpenCvSharp.Rect(left, top, right - left, bottom - top);
        }

        public void Calculate(RenderContext renderContext)
        {
            _renderImage = null;
#if DEBUG
            var stopwatch = new Stopwatch();
#endif

            //  var deviceDrawingArea = new Rect(0, 0, renderContext.RenderSize.Width , renderContext.RenderSize.Height);
            var deviceDrawingArea = new Rect(-renderContext.RenderSize.Width / 2, -renderContext.RenderSize.Height / 2, renderContext.RenderSize.Width * 2, renderContext.RenderSize.Height * 2);//2倍区域绘制,拖拽时的缓存效果
            deviceDrawingArea.Intersect(ImageDeviceRect);
            //绘制到设备的区域
            var renderDeviceArea = deviceDrawingArea;
            //绘制的Image像素区域
            var renderImagePixelArea = RenderTransform.ToDomain(renderDeviceArea);

            //绘制的Image像素区域
            var roundedImagePixelRect = GetRoundedImagePixelArea(_mat.Width, _mat.Height, renderImagePixelArea);

            var finalRoundedImagePixelRect = new Rect(roundedImagePixelRect.X, roundedImagePixelRect.Y, roundedImagePixelRect.Width, roundedImagePixelRect.Height);
            //剪裁图片
            using var clippedMat = new Mat(_mat, roundedImagePixelRect);
            var deviceDrawingWidth = deviceDrawingArea.Width < 1 ? 1 : deviceDrawingArea.Width;
            var deviceDrawingHeight = deviceDrawingArea.Height < 1 ? 1 : deviceDrawingArea.Height;

            using var resizeMat = new Mat();
            var drawSize = new OpenCvSharp.Size(deviceDrawingWidth, deviceDrawingHeight);

            try
            {
#if DEBUG
                // 开始计时
                stopwatch.Restart();
#endif
                Cv2.Resize(clippedMat, resizeMat, drawSize, 0, 0, InterpolationFlags.Area);
#if DEBUG
                stopwatch.Stop();
                Console.WriteLine($"resize,耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


#if DEBUG
            // 开始计时
            // stopwatch.Restart();
#endif
            _renderImage = resizeMat.ToWriteableBitmap();
            _renderImage.Freeze();
            _renderImageRect = finalRoundedImagePixelRect;
            _transformMatrix = renderContext.TransformMatrix;
#if DEBUG
            //stopwatch.Stop();
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
        private Rect _renderImageRect;
        private Matrix _transformMatrix;
        private ICoordTransform _coordTransformCache;



        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _mat.Dispose();
        }

        //todo:拖拽时的缓存效果
    }
}
