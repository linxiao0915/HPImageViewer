using HPImageViewer.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPImageViewer.Core
{
    public interface IHPImageViewer
    {
        public ImageViewerDesc ImageViewerDesc { get; set; }
        public void SetImage(object image);
    }
}
