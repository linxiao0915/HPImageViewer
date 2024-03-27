using System.Collections.Generic;
using HPImageViewer.Core.Primitives;

namespace HPImageViewer.Core.Persistence
{
    public class PolygonDesc : ROIDesc
    {
        public List<Point> Vertices { get; set; } = new List<Point>();
        ///// <summary>
        ///// 是否封闭图形
        ///// </summary>
        //public bool IsClosed { get; set; }
        public override string ToString()
        {
            string str = "";
            foreach (var vertice in Vertices)
            {
                str += $"{vertice}" + "; ";
            }
            return str;
        }
    }
}
