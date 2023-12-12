using HPImageViewer.Core.Persistence;
using HPImageViewer.Models;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HPImageViewer
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : UserControl, IImageViewer
    {
        public ImageControl()
        {
            InitializeComponent();
        }


        public void InvalidImageViewer()
        {
            // ImageViewDrawCanvas.
        }

        public void SetDocument(ImageViewerDesc imageViewerDesc)
        {
            ImageViewDrawCanvas.SetDocument(imageViewerDesc);
        }

        public void SetImage(object image)
        {
            Mat mat = null;
            if (image is Mat)
            {
                mat = (Mat)image;
            }
            else if (image is BitmapImage bitmapImage)
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapImage);
                mat = writeableBitmap.ToMat();
            }

            if (mat == null) return;
            ImageViewDrawCanvas.Image = mat;
        }


        private void ImageControl_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void ResetView_Click(object sender, RoutedEventArgs e)
        {
            ImageViewDrawCanvas.ResetView();
            ImageViewDrawCanvas.InvalidateVisual();
        }
    }
}
