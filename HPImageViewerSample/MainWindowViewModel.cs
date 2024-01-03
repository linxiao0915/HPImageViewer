using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HPImageViewer.Core;
using HPImageViewer.Core.Persistence;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HPImageViewerSample
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(IHPImageViewer imageViewer)
        {
            _imageViewer = imageViewer;
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
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName, UriKind.Relative));
                    bitmap.Freeze();
                    //var image = Cv2.ImRead(selectedFileName);
                    _imageViewer.FitNewImageToArea = true;
                    _imageViewer.SetImage(bitmap);

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

    }
}
