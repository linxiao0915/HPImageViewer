
using HPImageViewer.Core.Persistence;

namespace HPImageViewer.Core
{
    public interface IDocument
    {
        ImageViewerDesc ImageViewerDesc { get; set; }

        event EventHandler DocumentUpdated;

    }
}
