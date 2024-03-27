using HPImageViewer.Core.Persistence;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace HPImageViewer
{
    /// <summary>
    /// Interaction logic for ImageBindableViewer.xaml
    /// </summary>
    public partial class ImageBindableViewer : UserControl
    {
        public ImageBindableViewer()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                this.DataContextChanged += StageControlImageView_DataContextChanged;
            }

        }

        ImageBindableViewModel ViewModel => DataContext as ImageBindableViewModel;



        private void StageControlImageViewModel_ImageChanged(object sender, object e)
        {
            UpdateImage(e);

        }

        private void StageControlImageView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (e.OldValue is ImageBindableViewModel oldViewModel)
            {
                oldViewModel.ImageViewer = null;
                //ignore
            }

            if (e.NewValue is ImageBindableViewModel newViewModel)
            {
                newViewModel.ImageViewer = this.ImageViewer;
                UpdateBinding();
            }
        }

        private void StageControlImageView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateBinding();
        }

        private void UpdateBinding()
        {
            if (ViewModel == null) return;
            if (this.IsVisible)
            {
                UpdateImage(ViewModel.ImageData);
                UpdateROIs(ViewModel.Rois);
                ViewModel.ImageChanged -= StageControlImageViewModel_ImageChanged;
                ViewModel.ImageChanged += StageControlImageViewModel_ImageChanged;
                ViewModel.ROIsChanged -= ViewModel_ROIsChanged;
                ViewModel.ROIsChanged += ViewModel_ROIsChanged;
            }
            else
            {
                ViewModel.ImageChanged -= StageControlImageViewModel_ImageChanged;
                ViewModel.ROIsChanged -= ViewModel_ROIsChanged;
            }

        }

        private void ViewModel_ROIsChanged(object sender, List<ROIDesc> e)
        {
            UpdateROIs(e);
        }

        private void UpdateImage(object imageData)
        {
            if (imageData == null) return;
            ImageViewer.SetImage(imageData);
        }
        private void UpdateROIs(List<ROIDesc> rois)
        {
            ImageViewer.UpdateROIs(rois?.ToArray());
        }

    }
}
