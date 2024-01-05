using HPImageViewer.Core;
using HPImageViewer.Core.Miscs;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Tools;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        }
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private double _lastTime = 0.0d;
        private double _lowestFrameTime = double.MaxValue;
        private void CompositionTarget_Rendering(object? sender, EventArgs e)
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

            ConstructConversionTransformBlock();

        }

        private void InitializeCommands()
        {
            ResetViewCommand = new ImageViewerCommand(FitImageToArea);

            DeleteCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenders.GetSelectedROIs();
                if (selectedROIs.Any())
                {
                    foreach (var selectedROI in selectedROIs)
                    {
                        ImageViewDrawCanvas.ROIRenders.Remove(selectedROI);
                    }
                    ImageViewDrawCanvas.Rerender();
                }
            });

            SelectAllCommand = new ImageViewerCommand(() =>
            {
                foreach (var roiRender in ImageViewDrawCanvas.ROIRenders)
                {
                    roiRender.IsSelected = true;
                }
                ImageViewDrawCanvas.Rerender();
            });

            MoveToFrontCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenders.GetSelectedROIs();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenders.Remove(n));
                selectedROIs.Reverse();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenders.Insert(0, n));

                ImageViewDrawCanvas.Rerender();

            }, () => ImageViewDrawCanvas.ROIRenders.GetSelectedROIs().Count > 0);

            MoveToBackCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenders.GetSelectedROIs();
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenders.Remove(n));
                selectedROIs.ForEach(n => ImageViewDrawCanvas.ROIRenders.Add(n));
                ImageViewDrawCanvas.Rerender();
            }, () => ImageViewDrawCanvas.ROIRenders.GetSelectedROIs().Count > 0);
        }


        public ImageViewerDesc ImageViewerDesc
        {
            get => ImageViewDrawCanvas.ImageViewerDesc;
            set => ImageViewDrawCanvas.ImageViewerDesc = value;
        }
        DataTrafficLimiter _dataTrafficLimiter = new(100, 512 * 512 * 5);
        public void SetImage(object image)
        {
            //todo:此处加时间戳,并发下也有不严谨的，但是精确度不需要那么高，暂不考虑了；
            var value = 0;
            ConversionItem conversionItem = null;
            if (image is Mat matImage)
            {
                value = matImage.Width * matImage.Height;

                conversionItem = new ConversionItem(image) { MatConverter = (obj) => obj as Mat };
            }
            else if (image is BitmapImage bitmapImage)
            {
                value = (int)(bitmapImage.Width * bitmapImage.Height);
                conversionItem = new ConversionItem(image) { MatConverter = obj => new WriteableBitmap(obj as BitmapImage).ToMat() };
            }
            else if (image is Bitmap bitmap)
            {
                value = bitmap.Width * bitmap.Height;
                conversionItem = new ConversionItem(image) { MatConverter = obj => (obj as Bitmap).ToMat() };
            }
            else if (image is ConversionItem conversion)
            {
                conversionItem = conversion;
            }
            //todo:可能存在一个bug，最后一帧得不到显示，因为可能超过流量，need improvement
            if (_dataTrafficLimiter.TryAdd(value))
            {
                if (conversionItem != null)
                {
                    conversionItem.TimestampTick = _stopwatch.ElapsedTicks;
                }

                _imageConversionActionBlock.Post(conversionItem);
            }


        }

        public void AddROIs(params ROIDesc[] rois)
        {
            ImageViewDrawCanvas.AddROIs(rois);
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

        private ActionBlock<ConversionItem> _imageConversionActionBlock;

        private long _currentOutputTimestampTick = 0;
        private readonly object _syncLock = new object();
        private void ConstructConversionTransformBlock()
        {
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions();
            executionDataflowBlockOptions.EnsureOrdered = true;
            executionDataflowBlockOptions.MaxDegreeOfParallelism = -1;
            executionDataflowBlockOptions.BoundedCapacity = -1;

            _imageConversionActionBlock = new ActionBlock<ConversionItem>(item =>
            {

                Mat mat = null;
                if (item?.MatConverter != null)
                {
                    var image = item.Image;
                    mat = item.MatConverter(image);
                }

                lock (_syncLock)
                {
                    if (item?.TimestampTick > _currentOutputTimestampTick)
                    {
                        _currentOutputTimestampTick = item.TimestampTick;
                    }
                }

                ImageViewDrawCanvas.Image = mat;


            }, executionDataflowBlockOptions);

        }






    }
}
