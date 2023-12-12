using HPImageViewer.Core.Persistence;

namespace HPImageViewer.Models
{
    public interface IImageViewer
    {

        public void InvalidImageViewer();

        public void SetDocument(ImageViewerDesc imageViewerDesc);

        public void SetImage(object image);

    }
}
