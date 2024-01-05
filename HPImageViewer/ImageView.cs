using HPImageViewer.Core;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Rendering;
using HPImageViewer.Rendering.Layers;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Rect = System.Windows.Rect;

namespace HPImageViewer
{
    internal class ImageView : FrameworkElement, IDrawingCanvas, IDocument
    {

        public ImageView()
        {

            _IBackgroundLayers = new List<IBackgroundLayer>()
            {
                new GridBackgroundLayer(),
                new CrossHairLayer(),
                new ViewingInfoLayer(),
            };

            _renderEngine = new RenderEngine();
            _renderEngine.RenderRequested += _renderEngine_RenderRequested;

            SetDocument(new ImageViewerDesc());

            //_ROIRender.Add(new RectangleRender(new RectangleDesc()));

        }

        private void _renderEngine_RenderRequested(object? sender, ImageRender e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                ImageRender = e;
                InvalidateVisual();
            }, DispatcherPriority.Send);
        }

        private readonly RenderEngine _renderEngine;
        readonly List<IBackgroundLayer> _IBackgroundLayers;

        public ROIRenderCollection ROIRenders { get; private set; }

        public ImageRender ImageRender
        {
            get; set;

        }



        public void Rerender(Rect? affectedArea = null, bool immediate = true)
        {
            if (RenderSize.Height <= 0 || RenderSize.Width <= 0) return;

            if (affectedArea.HasValue == false)
            {
                if (immediate)
                {
                    if (CheckAccess())
                    {
                        InvalidateVisual();
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(InvalidateVisual);
                    }
                }

#if DEBUG
                //// 创建 Stopwatch 实例
                //var stopwatch = new Stopwatch();
                //// 开始计时
                //stopwatch.Restart();
#endif
                if (Image != null)//临时代码，由于目前只渲染image data
                {
                    var renderSession = new RenderSession(this, GetRenderContext(null));
                    _renderEngine.PostRenderSession(renderSession);
                }


#if DEBUG
                //stopwatch.Stop();
                //Console.WriteLine($"启动RenderTask 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif
                //GetRenderContext(null)
                // InvalidateVisual();//full render
            }

            //todo: mark render with dirty flag，partial rendering
        }

        public double Scale => TransformMatrix.M11;
        public Matrix TransformMatrix { get; private set; } = Matrix.Identity;// matrix是值类型，get会得到全新的
        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            var matrix = TransformMatrix;
            matrix.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformMatrix = matrix;
        }

        public void PanTo(double x, double y)
        {
            var matrix = TransformMatrix;
            matrix.Translate(x, y);
            TransformMatrix = matrix;
        }

        private RenderTransform _RenderTransform;
        public ICoordTransform CoordTransform
        {
            get
            {
                _RenderTransform ??= new RenderTransform(TransformMatrix);
                _RenderTransform.Matrix = TransformMatrix;
                return _RenderTransform;
            }
        }

        private Mat _image;
        public Mat Image
        {
            get => _image;
            set => SetImage(value);
        }

        private void SetImage(Mat image)
        {
            if (image == null) return;
            _image = image;
            if (FitNewImageToArea)
            {
                FitImageToArea();
            }

            Rerender(immediate: false);
        }

        public bool FitNewImageToArea { get; set; } = false;
        public void FitImageToArea()
        {
            if (Image == null) return;
            var imageWidth = Image.Width;
            var imageHeight = Image.Height;
            var areaWidth = RenderSize.Width;
            var areaHeight = RenderSize.Height;

            var widthScale = areaWidth / imageWidth;
            var heightScale = areaHeight / imageHeight;

            var imageZoomingScale = Math.Min(widthScale, heightScale);

            var transformMatrix = Matrix.Identity;
            transformMatrix.Translate((areaWidth - imageWidth) / 2, (areaHeight - imageHeight) / 2);
            transformMatrix.ScaleAt(imageZoomingScale, imageZoomingScale, areaWidth / 2, areaHeight / 2);
            TransformMatrix = transformMatrix;
        }





        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.</summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var renderContext = GetRenderContext(drawingContext);
            _IBackgroundLayers.ForEach(n => n.Render(renderContext));
            ImageRender?.Render(renderContext);
            foreach (var roiRender in ROIRenders)
            {
                roiRender.Render(renderContext);
            }
        }

        private ImageViewerDesc _imageViewerDesc;
        public ImageViewerDesc ImageViewerDesc
        {
            get => _imageViewerDesc;
            set
            {
                SetDocument(value);
            }
        }

        void SetDocument(ImageViewerDesc imageViewerDesc)
        {
            _imageViewerDesc = imageViewerDesc;
            ROIRenders = ROIRenderCollection.CreateByROIDescs(imageViewerDesc.ROIDescs, CoordTransform);
            Rerender();
        }

        public void AddROIs(params ROIDesc[] rois)
        {
            foreach (var roiDesc in ROIRenderCollection.CreateByROIDescs(rois.ToList(), CoordTransform))
            {
                ROIRenders.Add(roiDesc);
            }
            Rerender();

        }

        private RenderContext GetRenderContext(DrawingContext drawingContext)
        {
            var renderContext = new RenderContext(new WPFDrawingContext(drawingContext)) { Scale = this.Scale, TransformMatrix = this.TransformMatrix, RenderSize = RenderSize };
            renderContext.Image = Image;
            return renderContext;
        }


    }
}
