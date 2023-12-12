namespace HPImageViewer.Core.Persistence
{
    [Serializable]
    public class RectangleDesc : ROIDesc
    {
        public double Left
        {
            get;
            set;
        } = 100d;
        public double Top
        {
            get;
            set;
        } = 100d;

        public double Width
        {
            get;
            set;
        } = 100d;
        public double Height
        {
            get; set;
        } = 100d;
    }
}
