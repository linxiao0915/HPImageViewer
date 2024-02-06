using HalconDotNet;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HPImageViewer.Miscs
{
    internal static class ImageConverter
    {

        public static unsafe WriteableBitmap ToWriteableBitmap(this HObject hObject)
        {
            var watch = Stopwatch.StartNew();
            bool isSuccess = true;

            //初始化HObject，使其可以作为Halcon算子的输入/输出参数
            HObject dataHObject;
            HOperatorSet.GenEmptyObj(out dataHObject);
            dataHObject.Dispose();

            //获取通道数
            HOperatorSet.CountChannels(hObject, out var channels);
            if (channels.ToIArr().Length == 0)
            {
                throw new ArgumentException("输入 HObject 的通道数为0！可能是算法输出的结果为空！");
            }
            try
            {
                switch (channels.I)
                {
                    case 1:
                        //  pixelFormat = PixelFormat.Format8bppIndexed;
                        dataHObject = hObject;
                        break;

                    case 3:
                        //    pixelFormat = PixelFormat.Format24bppRgb;
                        //生成交错格式图片
                        HOperatorSet.InterleaveChannels(hObject, out dataHObject, "rgb", "match", 0);
                        break;

                    default:
                        throw new ArgumentException("输入 HObject 的通道数不受支持! 目前仅支持单通道和三通道! ");
                }

                //获取交错格式图像指针和宽高
                HOperatorSet.GetImagePointer1(dataHObject, out var hPointer, out var type, out _, out _);
                //注意：若通道数大于1
                //则此处GetImagePointer1的入参interleavedHObject是由InterleaveChannels生成的交错图像
                //故此处GetImagePointer1的出参width是生成的交错图像的实际宽度乘以其每像素字节数

                HOperatorSet.GetImageSize(hObject, out var width, out var height);
                var pixelFormats = channels.I == 1 ? PixelFormats.Gray8 : PixelFormats.Bgr24;

                var dest = new WriteableBitmap((int)width, (int)height, 96, 96, pixelFormats, null);
                dest.Lock();
                var sourceSpan = new Span<byte>(hPointer.IP.ToPointer(), dest.BackBufferStride * dest.PixelHeight);
                var destSpan = new Span<byte>(dest.BackBuffer.ToPointer(), dest.BackBufferStride * dest.PixelHeight);
                sourceSpan.CopyTo(destSpan);
                dest.AddDirtyRect(new Int32Rect(0, 0, dest.PixelWidth, dest.PixelHeight));
                dest.Unlock();

                return dest;
            }
            catch (Exception e)
            {
                isSuccess = false;
                throw;
            }
            finally
            {
                if (channels.I > 1)
                {
                    dataHObject.Dispose();
                }
                watch.Stop();
            }
        }
    }
}
