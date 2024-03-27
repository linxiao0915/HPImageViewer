using HalconDotNet;
using HPImageViewer.Core;
using HPImageViewer.Extensions.ImageDataIndexers;
using OpenCvSharp;
using System.Drawing;

namespace HPImageViewer.Extensions
{
    public class DefaultIndexerFactory : IPixelIndexerFactory
    {
        public PixelDataIndexer CreatePixelDataIndexer(object image)
        {
            switch (image)
            {
                case Mat matImage:
                    {
                        var imageDataIndexer = new HPImageViewer.Extensions.ImageDataIndexers.MatIndexer(matImage)/* { MatConverter = (obj) => obj as Mat }*/;
                        return imageDataIndexer;
                    }
                case HObject hImage:
                    {
                        var imageDataIndexer = new HalconPixelIndexer(hImage);
                        return imageDataIndexer;
                    }
                case Bitmap bitmap:
                    {
                        var imageDataIndexer = new BitmapIndexer(bitmap);
                        return imageDataIndexer;
                    }
                default:
                    return null;
            }
        }
    }



}
