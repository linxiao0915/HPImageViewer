namespace HPImageViewer.Core.Persistence
{
    public class RotatedRectDesc : ROIDesc
    {

        public double CenterX
        {
            get;
            set;
        } = 100d;

        public double CenterY
        {
            get;
            set;
        } = 100d;

        public double Angle
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        } = 100d;

        public double Height
        {
            get; set;
        } = 50;

    }
}
