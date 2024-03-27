using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPImageViewer.Core.Persistence
{
    public class BoxDesc : RectangleDesc
    {
        //ROI宽度
        public double BandWidth { get; set; } = 10;
    }
}
