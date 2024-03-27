using System;
using System.Collections.Generic;
using HalconDotNet;
using HPImageViewer.Core;
using HPImageViewer.Core.Primitives;

namespace HPImageViewer.Extensions.ImageDataIndexers
{
    public class HalconPixelIndexer : PixelDataIndexer
    {
        public HalconPixelIndexer(HObject hObject) : base(hObject)
        {
            HOperatorSet.CountChannels(hObject, out var channels);
            ChannelCount = channels.I;

            _hPointers = new List<IntPtr>();
            switch (channels.I)
            {
                case 1:
                    HOperatorSet.GetImagePointer1(hObject, out var hPointer, out _, out var width, out var height);
                    ImageSize = new Size(width, height);
                    _hPointers.Add(hPointer.IP);
                    break;
                case 3:
                    HOperatorSet.GetImagePointer3(hObject, out var hr, out var hg, out var hb, out _, out var width2, out var height2);
                    ImageSize = new Size(width2, height2);
                    _hPointers.Add(hr.IP);
                    _hPointers.Add(hg.IP);
                    _hPointers.Add(hb.IP);
                    break;

                default:
                    throw new ArgumentException("输入 HObject 的通道数不受支持! 目前仅支持单通道和三通道! ");
            }
            _stride = (int)ImageSize.Width;
        }
        private List<IntPtr> _hPointers;

        public override Size ImageSize { get; }

        public override int ChannelCount { get; }
        private int _stride;
        //  var strideSrc = GetStride(198659/*channels.I == 1 ? 198659 : 137224*/, width);
        public override byte GetPixelData(int channel, int row, int col)
        {
            unsafe
            {
                var offset = (long)row * (long)_stride + (long)col;
                return *((byte*)_hPointers[channel].ToPointer() + offset);//Select是性能杀手

            }
        }

    }
}
