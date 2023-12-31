using HPImageViewer.Core;
using HPImageViewer.Core.Miscs;
using HPImageViewer.Core.Persistence;
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
            ResetViewCommand = new ImageViewerCommand(() =>
            {
                ImageViewDrawCanvas.ResetView();

            });

            DeleteCommand = new ImageViewerCommand(() =>
            {
                var selectedROIs = ImageViewDrawCanvas.ROIRenders.Where(n => n.IsSelected).ToList();
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
            ConstructConversionTransformBlock();
        }


        public ImageViewerDesc ImageViewerDesc
        {
            get => ImageViewDrawCanvas.ImageViewerDesc;
            set => ImageViewDrawCanvas.ImageViewerDesc = value;
        }
        DataTrafficLimiter _dataTrafficLimiter = new(100, 512 * 512 * 5);
        public void SetImage(object image)
        {
            //todo:此次加时间戳,并发下也有不严谨的，但是精确度不需要那么高，暂不考虑了；
            var value = 0;
            if (image is Mat matImage)
            {
                value = matImage.Width * matImage.Height;
            }
            else if (image is BitmapImage bitmapImage)
            {
                value = (int)(bitmapImage.Width * bitmapImage.Height);

            }
            else if (image is Bitmap bitmap)
            {
                value = bitmap.Width * bitmap.Height;
            }
            //todo:可能存在一个bug，最后一帧得不到显示，因为可能超过流量，need improvement
            if (_dataTrafficLimiter.TryAdd(value))
            {
                _imageConversionActionBlock.Post(new ConversionItem(_stopwatch.ElapsedTicks, image));
            }


        }

        public void FitImageToArea()
        {
            ImageViewDrawCanvas.FitImageToArea();
        }


        public ICommand ResetViewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectAllCommand { get; private set; }


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
                var image = item.Image;
                Mat mat = null;
                if (image is Mat matImage)
                {
                    mat = matImage;
                }
                else if (image is BitmapImage bitmapImage)
                {
                    var writeableBitmap = new WriteableBitmap(bitmapImage);
                    mat = writeableBitmap.ToMat();
                }
                else if (image is Bitmap bitmap)
                {
                    mat = bitmap.ToMat();
                }

                lock (_syncLock)
                {
                    if (item.TimestampTick <= _currentOutputTimestampTick)
                        return;
                    _currentOutputTimestampTick = item.TimestampTick;
                }

                ImageViewDrawCanvas.Image = mat;


            }, executionDataflowBlockOptions);

        }

        class ConversionItem
        {
            public long TimestampTick { get; }
            public object Image { get; set; }

            public ConversionItem(long timestampTick, object image)
            {
                TimestampTick = timestampTick;
                Image = image;
            }
        }




    }
}
