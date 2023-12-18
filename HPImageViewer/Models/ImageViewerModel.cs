using CommunityToolkit.Mvvm.ComponentModel;
using HPImageViewer.Core.Persistence;

namespace HPImageViewer.Models
{
    public partial class ImageControlViewModel : ObservableObject, IImageViewer
    {
        public ImageControlViewModel()
        {

        }

        IDrawingCanvas _drawingCanvas;

        public void InvalidImageViewer()
        {
            _drawingCanvas?.Rerender();
        }

        public void SetDocument(ImageViewerDesc imageViewerDesc)
        {


        }

        public void SetImage(object image)
        {

        }

        private void ExecuteOpenImage()
        {
            OnPropertyChanged(nameof(ExecuteOpenImage));
        }


    }
}
