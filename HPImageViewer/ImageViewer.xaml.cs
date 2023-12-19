using System;
using HPImageViewer.Core.Persistence;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HPImageViewer.Core;

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

        }




        public ImageViewerDesc ImageViewerDesc
        {
            get => ImageViewDrawCanvas.ImageViewerDesc;
            set => ImageViewDrawCanvas.ImageViewerDesc = value;
        }

        public void SetImage(object image)
        {
            lock (_lockObj)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                SetImageAsync(image, _cancellationTokenSource.Token);
            }

        }

        private object _lockObj = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private async Task SetImageAsync(object image, CancellationToken cancellationToken)
        {

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            Mat mat = null;
            try
            {
                await Task.Run(() =>
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    if (image is Mat)
                    {
                        mat = (Mat)image;
                    }
                    else if (image is BitmapImage bitmapImage)
                    {
                        var writeableBitmap = new WriteableBitmap(bitmapImage);
                        mat = writeableBitmap.ToMat();
                    }

                }, cancellationToken);
                if (mat == null) return;
                ImageViewDrawCanvas.Image = mat;
            }
            catch (TaskCanceledException taskCanceledException)
            {

            }



        }




        private void ImageControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void ResetView_Click(object sender, RoutedEventArgs e)
        {

        }

        public ICommand ResetViewCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand SelectAllCommand { get; private set; }


    }
}
