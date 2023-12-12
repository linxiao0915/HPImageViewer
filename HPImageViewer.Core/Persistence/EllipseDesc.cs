namespace HPImageViewer.Core.Persistence
{
    public class EllipseDesc : ROIDesc
    {
        public EllipseDesc()
        {

        }
        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double R { get; set; } = 10d;

    }
}
