using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HPImageViewer.Models;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HPImageViewerSample
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel(IImageViewer imageViewer)
        {
            _imageViewer = imageViewer;
        }
        private IImageViewer _imageViewer;
        [RelayCommand]
        private void ExecuteOpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*"; // 设置筛选文件类型
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFileName = openFileDialog.FileName;

                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFileName, UriKind.Relative));

                    //var image = Cv2.ImRead(selectedFileName);
                    _imageViewer.SetImage(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载图片出错：" + ex.Message);
                }
            }


        }


    }
}
