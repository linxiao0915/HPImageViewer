using System;
using HPImageViewer.Core;
using OpenCvSharp;


namespace HPImageViewer.Extensions.ImageDataIndexers
{
    public class MatIndexer : PixelDataIndexer
    {
        Mat.UnsafeIndexer<Vec3b> _unsafeIndexer;
        public MatIndexer(Mat mat) : base(mat)
        {
            ImageSize = new HPImageViewer.Core.Primitives.Size(mat.Width, mat.Height);
            ChannelCount = mat.Channels();
            _unsafeIndexer = mat.GetUnsafeGenericIndexer<Vec3b>();
        }

        public override HPImageViewer.Core.Primitives.Size ImageSize { get; }
        public override int ChannelCount { get; }
        public override byte GetPixelData(int channel, int row, int col)
        {
            var value = _unsafeIndexer[row, col][channel];
            GC.KeepAlive(Image);
            return value;

        }
    }
}
