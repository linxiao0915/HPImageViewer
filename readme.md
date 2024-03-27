# HPImageViewer

目标高性能的图像显示、ROI绘制控件，使用M-V-C模式构建

## Supported Platforms

.net framework 4.8 

.net 6

.net 7

## 支持的图像数据类型

OpenCV的Mat的灰度和三通道（rgb）

Halcon的HObject的灰度和三通道（rgb）

Bitmapt的灰度和三通道（rgb）

自定义的数据类型，通过实现自定义的像素索引器：

```C#

namespace HPImageViewer.Core
{
    public abstract class PixelDataIndexer : IDisposable
    {
        ...
    }
}

 public class HalconPixelIndexer : PixelDataIndexer 
 public class MatIndexer : PixelDataIndexer
 public class BitmapIndexer : PixelDataIndexer
```









## 支持的ROI类型

Box

Ellipse

Polygon

QuadRectangle

Rectangle

RotatedRect



## 项目结构

HPImageViewer.Core roi 接口定义，数据类型定义，持久化数据结构定义

HPImageViewer 基于WPF的绘制实现

HPImageViewer.Extensions 包含自定义PixelIndexer的扩展

ImageBindingDisplay 对ROIs和图像数据通过MVVM方式使用HPImageViewer的封装，支持绑定使用



## 使用方式

    public interface IDocument
    {
       ImageViewerDesc ImageViewerDesc { get; set; }//文档模型，set该属性会被控件感知，修改显示内容
       
       event EventHandler DocumentUpdated;//文档更新时触发该事件
    
    }
    
    public interface IHPImageViewer : IDocument
    {
        public object Image { get; set; }//更新显示的图像
    
        public void AddROIs(params ROIDesc[] rois);//添加显示的ROI
        public void UpdateROIs(params ROIDesc[] rois);//更新显示的ROI
        bool FitNewImageToArea { get; set; }//更新图片时是否适应到视口
    
        void FitImageToArea();//当前图片适应到视口区域
    
        public ToolType ActivatedTool { get; set; }//使用的绘制或交互功能
    
        event EventHandler ShapeDrawCompleted;//用户绘制完成触发该事件
    
    }



## 性能



支持图片大小：

测试可以正常流畅地显示52019*52032以上图像，上限未知；

受限于设备（计算机位数、应用程序内存空间）的存储和数值计算的溢出；



ROI：

上限未知，尚有优化空间，可根据具体项目需要进一步优化



![](https://github.com/guyyoulove/HPImageViewer/blob/develop/%E5%A4%A7%E5%9B%BE.png)





