namespace HPImageViewer.Core.Persistence
{
    public interface IDocument
    {
        ImageViewerDesc ImageViewerDesc { get; }
        public void SetDocument(ImageViewerDesc imageViewerDesc);

    }
}
