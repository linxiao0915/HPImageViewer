using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HalconDotNet;
using HPImageViewer.Core;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Extensions.Extensions;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Point = HPImageViewer.Core.Primitives.Point;

namespace HPImageViewerSample
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(IHPImageViewer imageViewer)
        {
            AggregationIndexerFactory.Instance.RegisterDefaultIndexerFactory();
            _imageViewer = imageViewer;
            _imageViewer.ImageViewerDesc = new ImageViewerDesc()
            {
                ROIDescs = new List<ROIDesc>()
                {
                    //new RotatedRectDesc(){Angle=30},
                  //  new RotatedRectDesc(){Angle=60},
                  //    new RotatedRectDesc(){Angle=90},
                  //new RotatedRectDesc(){Angle=120},
                  //new RotatedRectDesc(){Angle=150},
                   new BoxDesc(){Top=10,Left=10,Width=200,Height=200,BandWidth=30,Color="#FFFF00"},
                   new QuadRectangleDesc(){Top=10,Left=300,Width=200,Height=200,BandWidth=30,Color="#FFFF00",BandLength=80},
                }
            };
        }
        private IHPImageViewer _imageViewer;
        [RelayCommand]
        private void ExecuteOpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"; // 设置筛选文件类型
            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFileName = openFileDialog.FileName;

                try
                {
                    /*
                     BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName, UriKind.Relative));
                       bitmap.Freeze();

                       //var image = Cv2.ImRead(selectedFileName);

                        HOperatorSet.ReadImage(out var hImage, selectedFileName);
                       _imageViewer.FitNewImageToArea = true;
                       _imageViewer.SetImage(bitmap);

                    */

                    HOperatorSet.ReadImage(out var image, selectedFileName);
                    //_imageViewer.FitNewImageToArea = true;
                    //var bitmap = new Bitmap(openFileDialog.FileName);


                    //  var image = Cv2.ImRead(selectedFileName, ImreadModes.Grayscale);

                    _imageViewer.SetImage(image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载图片出错：" + ex.Message);
                }
            }

        }


        [RelayCommand]
        private void ExecuteOpenDocument()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ImageViewer文件 (*.IMVDesc)|*.IMVDesc"; // 设置筛选文件
            if (openFileDialog.ShowDialog() != true) return;
            try
            {
                var selectedFileName = openFileDialog.FileName;
                var imageViewerDescString = File.ReadAllText(selectedFileName);
                if (string.IsNullOrEmpty(imageViewerDescString)) return;
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var imageViewerDesc = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageViewerDesc>(imageViewerDescString, settings);
                if (imageViewerDesc == null) return;

                _imageViewer.ImageViewerDesc = imageViewerDesc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }



        }
        [RelayCommand]
        private void ExecuteSaveDocument()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "ImageViewer文件 (*.IMVDesc)|*.IMVDesc";
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            var fileName = saveFileDialog.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            try
            {
                var imageViewerDesc = _imageViewer.ImageViewerDesc;
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var imageViewerDescString = Newtonsoft.Json.JsonConvert.SerializeObject(imageViewerDesc, settings);
                if (string.IsNullOrEmpty(imageViewerDescString)) return;
                File.WriteAllText(fileName, imageViewerDescString);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        [RelayCommand]
        private void ExecuteChangeToolType(ToolType toolType)
        {
            _imageViewer.ActivatedTool = toolType;
        }

        [RelayCommand]
        private void ExecuteAddRoi(ROIType roiType)
        {
            ROIDesc roi = null;
            switch (roiType)
            {
                case ROIType.ToolRectangle:
                    roi = new RectangleDesc() { Left = 0, Top = 0, Width = 200, Height = 200 };
                    break;
                case ROIType.ToolEllipse:
                    roi = new EllipseDesc() { CenterX = 0, CenterY = 0, R = 100 };
                    break;
                case ROIType.ToolPolygon:
                    roi = new PolygonDesc()
                    {
                        Vertices = new List<Point>()
                        {
                            new Point(0,0),
                            new Point(100,50),
                            new Point(50,100),
                        }
                    };
                    break;
            }

            if (roi != null)
            {
                _imageViewer.AddROIs(roi);
            }

        }

    }

    public enum ROIType
    {
        ToolRectangle,
        ToolEllipse,
        ToolPolygon,
    }
}
