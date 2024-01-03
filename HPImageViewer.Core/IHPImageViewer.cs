
using System.Security.Permissions;

namespace HPImageViewer.Core
{
    public interface IHPImageViewer : IDocument
    {
        public void SetImage(object image);

        bool FitNewImageToArea { get; set; }
        void FitImageToArea();
    }
}
