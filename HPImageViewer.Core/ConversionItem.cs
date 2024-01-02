using OpenCvSharp;

namespace HPImageViewer.Core
{
    public class ConversionItem
    {
        public long TimestampTick { get; set; }
        public object Image { get; set; }

        public ConversionItem(object image)
        {

            Image = image;
        }


        public Func<object, Mat> MatConverter { get; set; }
    }
}
