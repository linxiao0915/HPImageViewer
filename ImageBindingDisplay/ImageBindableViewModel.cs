using HPImageViewer.Core;
using HPImageViewer.Core.Persistence;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HPImageViewer
{
    public class ImageBindableViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event EventHandler<object> ImageChanged;
        public event EventHandler<List<ROIDesc>> ROIsChanged;

        private object _imageData;
        public object ImageData
        {
            get => _imageData;
            set
            {   //临时代码
                _imageData = value;
                // var bitmap = _imageData.ToBitmap();
                //  BitmapCache = bitmap;
                ImageChanged?.Invoke(this, value);
            }
        }

        private List<ROIDesc> _rois = new List<ROIDesc>();
        public List<ROIDesc> Rois
        {
            get => _rois;
            set
            {   //临时代码
                _rois = value;
                // var bitmap = _imageData.ToBitmap();
                //  BitmapCache = bitmap;
                ROIsChanged?.Invoke(this, value);
            }
        }

        public IHPImageViewer ImageViewer { get; internal set; }


    }
}
