using HalconDotNet;
using HPImageViewer.Core;
using OpenCvSharp;
using System.Drawing;

namespace HPImageViewer.Extensions.Extensions
{
    public static class ImageViewerExtensions
    {
        public static AggregationIndexerFactory RegisterDefaultIndexerFactory(this AggregationIndexerFactory aggregationIndexerFactory)
        {
            return aggregationIndexerFactory
                    .Register<Bitmap, DefaultIndexerFactory>()
                    .Register<Mat, DefaultIndexerFactory>()
                    .Register<HObject, DefaultIndexerFactory>();

        }


    }
}
