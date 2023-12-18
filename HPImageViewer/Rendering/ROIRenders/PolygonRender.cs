using HPImageViewer.Core.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;


namespace HPImageViewer.Rendering.ROIRenders
{
    internal class PolygonRender : ROIRender
    {

        public PolygonDesc PolygonDesc
        {
            get => _rOIDesc as PolygonDesc;
            set => _rOIDesc = value;
        }

        public List<Point> Points
        {
            get => PolygonDesc.Vertices;
            set => PolygonDesc.Vertices = value;
        }

        public bool IsClosed
        {
            get;
            set;
        }
        public PolygonRender() : this(new PolygonDesc())
        {

        }

        public PolygonRender(PolygonDesc rOIDesc) : base(rOIDesc)
        {

        }

        protected override void OnROIRender(RenderContext renderContext)
        {
            if (Points?.Count > 1)
            {
                for (int i = 0; i < Points.Count - 1; i++)
                {

                    renderContext.DrawLine(new Pen(Brush, _rOIDesc.StrokeThickness), RenderTransform.ToDevice(Points[i]), RenderTransform.ToDevice(Points[i + 1]));

                }

                if (IsClosed)
                {
                    renderContext.DrawLine(new Pen(Brush, _rOIDesc.StrokeThickness), RenderTransform.ToDevice(Points[Points.Count - 1]), RenderTransform.ToDevice(Points[0]));
                }
            }


        }

        public override int HitTest(Point point)
        {//todo:凸凹多边形处理
            if (IsSelected)
            {

                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandlePoint(i).Contains(point))
                        return i;
                }
            }
            if (IsPointInPolygon(RenderTransform.ToDomain(point), PolygonDesc))
            {
                return 0;
            }

            return -1;

        }

        public override int HandleCount => Points.Count;
        protected override void MoveToInternal(double wx, double wy)
        {
            Points = Points.Select(s =>
               {
                   s.X += wx;
                   s.Y += wy;
                   return s;
               }).ToList();
        }

        protected override void MoveHandleToInteranl(int handleNumber, Point point)
        {

            Points[handleNumber - 1] = point;
        }

        public override Point GetHandle(int handleNumber)
        {
            return RenderTransform.ToDevice(Points[handleNumber - 1]);
        }

        public static bool IsPointInPolygon(Point point, PolygonDesc polygon)
        {
            int intersectionCount = 0;
            int n = polygon.Vertices.Count;

            for (int i = 0; i < n; i++)
            {
                Point p1 = polygon.Vertices[i];
                Point p2 = polygon.Vertices[(i + 1) % n];

                if (IsIntersect(point, p1, p2))
                {
                    intersectionCount++;
                }
            }

            return intersectionCount % 2 == 1;
        }

        private static bool IsIntersect(Point point, Point p1, Point p2)
        {
            if (point.Y > Math.Max(p1.Y, p2.Y) || point.Y <= Math.Min(p1.Y, p2.Y))
            {
                return false;
            }

            if (point.X <= Math.Max(p1.X, p2.X))
            {
                double xIntersection = (point.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;

                if (xIntersection > point.X)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
