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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(CenterX)}:{CenterX:N4},{nameof(CenterY)}:{CenterY:N4},{nameof(R)}:{R:N4}";
        }
    }
}
