using HPImageViewer.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using Size = HPImageViewer.Core.Primitives.Size;

namespace HPImageViewer.Extensions.ImageDataIndexers
{

    public class BitmapIndexer : PixelDataIndexer
    {
        BitmapData _bitmapData;
        //static Dictionary<Bitmap,BitmapData> _bitBitmapDatas=new Dictionary<Bitmap,BitmapData>();
        public BitmapIndexer(Bitmap bitmap) : base(bitmap)
        {
            Monitor.Enter(bitmap);
            ImageSize = new Size(bitmap.Width, bitmap.Height);
            ChannelCount = bitmap.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : 3;
            _bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

        }


        public override Size ImageSize { get; }
        public override int ChannelCount { get; }
        public override byte GetPixelData(int channel, int row, int col)
        {
            unsafe
            {
                //var bitmapData = _threadLocalBitmapData.Value;
                var offset = (long)row * (long)_bitmapData.Stride + (long)col * ChannelCount + ChannelCount - channel - 1;
                return *((byte*)_bitmapData.Scan0.ToPointer() + offset);
                //   return *(((byte*)(_bitmapData.Scan0.ToPointer()) + offset));
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public override void Dispose()
        {
            var bitmap = (Image as Bitmap);
            bitmap.UnlockBits(_bitmapData);
            Monitor.Exit(bitmap);
        }

    }
}
