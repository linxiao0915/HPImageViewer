using System;
using Size = HPImageViewer.Core.Primitives.Size;

namespace HPImageViewer.Core
{
    public abstract class PixelDataIndexer : IDisposable
    {
        public object Image { get; protected set; }
        public abstract Size ImageSize { get; }
        protected PixelDataIndexer(object image)
        {
            Image = image;
        }
        public virtual void LockData() { }
        public virtual void UnlockData() { }

        public abstract int ChannelCount { get; }

        public abstract byte GetPixelData(int channel, int row, int col);

        protected static int GetStride(int format, int width)
        {
            var bytesPerPixel = GetByteCountPerPixel(format);
            int stride = 4 * ((width * bytesPerPixel + 3) / 4);
            return stride;
        }

        protected static int GetByteCountPerPixel(int format)
        {
            var bitsPerPixel = ((int)format & 0xff00) >> 8;
            var bytesPerPixel = (bitsPerPixel + 7) / 8;
            return bytesPerPixel;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose()
        {
            Image = null;//表面上是释放，实际上却是延长image生命周期防止GC回收非托管的Image对象，必须加此代码，否则程序会崩溃
            // TODO release managed resources here
        }
    }





}
