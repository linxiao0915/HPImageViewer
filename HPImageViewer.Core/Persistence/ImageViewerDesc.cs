namespace HPImageViewer.Core.Persistence
{
    [Serializable]
    public class ImageViewerDesc
    {


        public ImageViewerDesc()
        {

        }

        public List<ROIDesc> ROIDescs
        {
            get;
            set;
        } = new List<ROIDesc>();



    }
}
