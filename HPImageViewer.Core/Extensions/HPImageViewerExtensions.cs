using OpenCvSharp;

namespace HPImageViewer.Core.Extensions
{
    public static class HPImageViewerExtensions
    {
        public static void SetImageData<T>(this IHPImageViewer imageViewer, T imageData, Func<T, Mat> matConverter)
        {
            imageViewer.SetImage(new ConversionItem(imageData) { MatConverter = obj => matConverter.Invoke((T)obj) });
        }


    }
}
