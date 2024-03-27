using System;

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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(Left)}:{Left:N4},{nameof(Top)}:{Top:N4},{nameof(Width)}:{Width:N4},{nameof(Height)}:{Height:N4}";
        }
    }


}
