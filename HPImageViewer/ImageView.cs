﻿using HPImageViewer.Core;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Rendering;
using HPImageViewer.Rendering.Layers;
using HPImageViewer.Rendering.ROIRenders;
using HPImageViewer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using HPImageViewer.RoutedEventArgs;
using Rect = System.Windows.Rect;
using Size = HPImageViewer.Core.Primitives.Size;

namespace HPImageViewer
{
    public class ImageView : FrameworkElement, IDrawingCanvas, IDocument
    {

        public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(ImageView), (PropertyMetadata)new FrameworkPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>Identifies the <see cref="P:System.Windows.Controls.Control.Foreground" /> dependency property.</summary>
        /// <returns>The brush that is used to fill the background of the control. The default is <see cref="P:System.Windows.Media.Brushes.Transparent" />.</returns>
        public Brush Background
        {
            get => (Brush)this.GetValue(ImageView.BackgroundProperty);
            set => this.SetValue(ImageView.BackgroundProperty, (object)value);
        }

        private LayerCollection _layerCollection = new LayerCollection();

        public ImageView()
        {


            _renderEngine = new RenderEngine();
            _renderEngine.RenderRequested += _renderEngine_RenderRequested;

            SetDocument(new ImageViewerDesc());

            //_ROIRender.Add(new RectangleRender(new RectangleDesc()));

        }

        public static readonly RoutedEvent DocumentUpdatedEvent = EventManager.RegisterRoutedEvent(nameof(DocumentUpdated), RoutingStrategy.Bubble, typeof(EventHandler), typeof(ImageView));

        public event EventHandler DocumentUpdated
        {
            add { AddHandler(DocumentUpdatedEvent, value); }
            remove { RemoveHandler(DocumentUpdatedEvent, value); }
        }


        public static readonly RoutedEvent ShapeDrawCompletedEvent = EventManager.RegisterRoutedEvent(nameof(ShapeDrawCompleted), RoutingStrategy.Bubble, typeof(EventHandler), typeof(ImageView));

        public event EventHandler ShapeDrawCompleted
        {
            add { AddHandler(ShapeDrawCompletedEvent, value); }
            remove { RemoveHandler(ShapeDrawCompletedEvent, value); }
        }


        public static readonly RoutedEvent ImageDoubleClickedEvent = EventManager.RegisterRoutedEvent(nameof(ImageDoubleClicked), RoutingStrategy.Bubble, typeof(EventHandler), typeof(ImageView));

        public event EventHandler<ImageDoubleClickedRoutedEventArgs> ImageDoubleClicked
        {
            add { AddHandler(ImageDoubleClickedEvent, value); }
            remove { RemoveHandler(ImageDoubleClickedEvent, value); }
        }



        private void _renderEngine_RenderRequested(object sender, RenderSet e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                UpdateUI(e);

            }, DispatcherPriority.Send);
        }
        private void UpdateUI(RenderSet renderSet)
        {
            ImageRender = renderSet.ImageRender;
            ImageSize = ImageRender?.ImageSize ?? new Size(0, 0);
            if (renderSet.RenderContext.FitNewImageToArea)
            {
                FitImageToArea();
            }
            InvalidateVisual();
        }
        private readonly RenderEngine _renderEngine;

        public ROIRenderCollection ROIRenderCollection { get; private set; }

        public ImageRender ImageRender
        {
            get; set;

        }

        public void Rerender(bool immediate = true, RenderContext renderContext = null)
        {
            if (RenderSize.Height <= 0 || RenderSize.Width <= 0) return;



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
            var renderSession = new RenderSession(this, renderContext ?? GetRenderContext(null));
            _renderEngine.PostRenderSession(renderSession);


#if DEBUG
            //stopwatch.Stop();
            //Console.WriteLine($"启动RenderTask 耗时:{stopwatch.Elapsed.TotalMilliseconds}");
#endif
            //GetRenderContext(null)
            // InvalidateVisual();//full render


            //todo: mark render with dirty flag，partial rendering
        }

        public double Scale => TransformMatrix.M11;
        public Matrix TransformMatrix { get; internal set; } = Matrix.Identity;// matrix是值类型，get会得到全新的
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

        private object _image;
        public object Image
        {
            get => _image;
            set => SetImage(value);
        }


        private void SetImage(object image)
        {
            _image = image;
            //ImageSize = new Size(pixel.ImageSize.Width, pixel.ImageSize.Height);//todo:放到session避免并发问题
            var renderContext = GetRenderContext(null);
            renderContext.FitNewImageToArea = FitNewImageToArea;
            renderContext.Image = image;
            Rerender(immediate: false, renderContext: renderContext);
        }

        public Size ImageSize { get; private set; }

        public bool FitNewImageToArea { get; set; } = false;
        public void FitImageToArea()
        {
            if (Image == null || ImageSize == null) return;
            var imageWidth = ImageSize.Width;
            var imageHeight = ImageSize.Height;
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
            RenderBackgroundColor(drawingContext);
            var renderContext = GetRenderContext(drawingContext);
            _layerCollection.BackgroundLayers.ForEach(n => n.Render(renderContext));

            ImageRender?.Render(renderContext);
            ROIRenderCollection.Render(renderContext);

            _layerCollection.ForegroundLayers.ForEach(n => n.Render(renderContext));
        }
        private void RenderBackgroundColor(DrawingContext drawingContext)
        {
            var background = this.Background;
            drawingContext.DrawRectangle(background, null, new Rect(0, 0, ActualWidth, ActualHeight));
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
            //todo:线程安全
            _imageViewerDesc = imageViewerDesc;
            UpdateROIs(imageViewerDesc.ROIDescs.ToArray());
            Rerender();
        }

        private void ROIRenderCollection_RoisChanged(object sender, EventArgs e)
        {
            this.RaiseEvent(new System.Windows.RoutedEventArgs(DocumentUpdatedEvent));
        }

        public void AddROIs(params ROIDesc[] rois)
        {
            foreach (var roiDesc in ROIRenderCollection.CreateByROIDescs(rois.ToList(), CoordTransform))
            {
                ROIRenderCollection.Add(roiDesc);
            }
            this.RaiseEvent(new System.Windows.RoutedEventArgs(DocumentUpdatedEvent));
            Rerender();

        }
        public void UpdateROIs(params ROIDesc[] rois)
        {
            if (ROIRenderCollection != null)
            {
                ROIRenderCollection.RoisChanged -= ROIRenderCollection_RoisChanged;
            }
            _imageViewerDesc.ROIDescs = rois?.ToList() ?? new List<ROIDesc>();
            ROIRenderCollection = ROIRenderCollection.CreateByROIDescs(_imageViewerDesc.ROIDescs, CoordTransform);
            ROIRenderCollection.RoisChanged += ROIRenderCollection_RoisChanged;
            Rerender();
        }

        private RenderContext GetRenderContext(DrawingContext drawingContext)
        {
            var renderContext = new RenderContext(new WPFDrawingContext(drawingContext)) { Scale = this.Scale, TransformMatrix = this.TransformMatrix, RenderSize = RenderSize };
            renderContext.Image = Image;
            renderContext.ImageSize = ImageSize;
            return renderContext;
        }


    }
}
