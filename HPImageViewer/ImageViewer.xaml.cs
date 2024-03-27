using HPImageViewer.Core;
using HPImageViewer.Core.Miscs;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Tools;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HPImageViewer
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageViewer : UserControl, IHPImageViewer
    {
        public ImageViewer()
        {
            Initialize();
            InitializeComponent();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            SetCurrentValue(BackgroundProperty, new SolidColorBrush(Colors.Black));
            AddHandler(ImageView.DocumentUpdatedEvent, new RoutedEventHandler(ImageViewDrawCanvas_DocumentUpdated));
            AddHandler(ImageView.ShapeDrawCompletedEvent, new RoutedEventHandler(ImageViewDrawCanvas_ShapeDrawCompleted));

        }


        private void ImageViewDrawCanvas_DocumentUpdated(object sender, EventArgs e)
        {
            DocumentUpdated?.Invoke(this, e);
        }
        public event EventHandler ShapeDrawCompleted;


        private void ImageViewDrawCanvas_ShapeDrawCompleted(object sender, EventArgs e)
        {
            ShapeDrawCompleted?.Invoke(sender, e);
        }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
        }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }


        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private double _lastTime = 0.0d;
        private double _lowestFrameTime = double.MaxValue;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            var timeNow = _stopwatch.ElapsedTicks;
            var elapsedMilliseconds = timeNow - _lastTime;
            _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
            FpsCounter.Text = $"FPS: {10000000.0 / elapsedMilliseconds:0.0} / Max: {10000000.0 / _lowestFrameTime:0.0}";
            _lastTime = timeNow;
        }
        void Initialize()
        {
            ActivatedTool = ToolType.ToolPan;
            InitializeCommands();

        }

        private void InitializeCommands()
        {
            ResetViewCommand = new ImageViewerCommand(() =>
            {
                if (ImageViewDrawCanvas.Image == null)
                {
                    ImageViewDrawCanvas.TransformMatrix = Matrix.Identity;
                    ImageViewDrawCanvas.Rerender();
                }
                else
                {
                    FitImageToArea();
                }

            });

            DeleteCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenderCollection.GetSelectedROIs();
                if (selectedROIs.Any())
                {
                    foreach (var selectedROI in selectedROIs)
                    {
                        ImageViewDrawCanvas.ROIRenderCollection.Remove(selectedROI);
                    }
                    ImageViewDrawCanvas.Rerender();
                }
            });

            SelectAllCommand = new ImageViewerCommand(() =>
            {
                foreach (var roiRender in ImageViewDrawCanvas.ROIRenderCollection)
                {
                    roiRender.IsSelected = true;
                }
                ImageViewDrawCanvas.Rerender();
            });

            MoveToFrontCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenderCollection.GetSelectedROIs();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenderCollection.Remove(n));
                selectedROIs.Reverse();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenderCollection.Insert(0, n));

                ImageViewDrawCanvas.Rerender();

            }, () => ImageViewDrawCanvas.ROIRenderCollection.GetSelectedROIs().Count > 0);

            MoveToBackCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenderCollection.GetSelectedROIs();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenderCollection.Remove(n));
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenderCollection.Add(n));
                ImageViewDrawCanvas.Rerender();
            }, () => ImageViewDrawCanvas.ROIRenderCollection.GetSelectedROIs().Count > 0);
        }


        public ImageViewerDesc ImageViewerDesc
        {
            get => ImageViewDrawCanvas.ImageViewerDesc;
            set => ImageViewDrawCanvas.ImageViewerDesc = value;
        }

        public event EventHandler DocumentUpdated;


        DataTrafficLimiter _dataTrafficLimiter = new(100, 512 * 512 * 5);
        public void SetImage(object Image)
        {
            //todo:此处加时间戳,并发下也有不严谨的，但是精确度不需要那么高，暂不考虑了；

            //else if (image is BitmapImage bitmapImage)
            //{
            //    value = (int)(bitmapImage.Width * bitmapImage.Height);
            //    conversionItem = new ConversionItem(image); /*{ MatConverter = obj => new WriteableBitmap(obj as BitmapImage).ToMat() };*/
            //}
            //else if (image is Bitmap bitmap)
            //{
            //    value = bitmap.Width * bitmap.Height;
            //    conversionItem = new ConversionItem(image) { MatConverter = obj => ((Bitmap)((Bitmap)obj).Clone()).ToMat() };
            //}

            this.ImageViewDrawCanvas.Image = Image;
            //todo:可能存在一个bug，最后一帧得不到显示，因为可能超过流量，need improvement
        }

        public object Image
        {
            get => this.ImageViewDrawCanvas.Image;
            set => this.ImageViewDrawCanvas.Image = value;
        }

        public void AddROIs(params ROIDesc[] rois)
        {
            ImageViewDrawCanvas.AddROIs(rois);
        }

        public void UpdateROIs(params ROIDesc[] rois)
        {
            ImageViewDrawCanvas.UpdateROIs(rois);
        }

        public bool FitNewImageToArea
        {
            get => ImageViewDrawCanvas.FitNewImageToArea;
            set => ImageViewDrawCanvas.FitNewImageToArea = value;
        }

        public void FitImageToArea()
        {
            ImageViewDrawCanvas.FitImageToArea();
            ImageViewDrawCanvas.Rerender();
        }
        private ToolType _activatedTool;
        public ToolType ActivatedTool
        {
            get => _activatedTool;
            set
            {
                if (_activatedTool != value)
                {
                    _activatedTool = value;
                    ActivatedToolInternal = GetToolByToolType(value);
                }

            }
        }
        //todo:切换时上下文的设置，对旧工具的卸载功能(还原现场)
        internal ITool ActivatedToolInternal { get; private set; }

        private ITool GetToolByToolType(ToolType toolType)
        {
            switch (toolType)
            {
                case ToolType.None:
                    return null;

                case ToolType.ToolPointer:
                    return new ToolPointer();
                case ToolType.ToolPan:
                    return new ToolPan();
                case ToolType.ToolRectangle:
                    return new ToolRectangle();
                case ToolType.ToolEllipse:
                    return new ToolEllipse();
                case ToolType.ToolPolygon:
                    return new ToolPolygon();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public ICommand ResetViewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectAllCommand { get; private set; }
        public ICommand MoveToFrontCommand { get; private set; }
        public ICommand MoveToBackCommand { get; private set; }

        private readonly object _syncLock = new object();


    }


}
