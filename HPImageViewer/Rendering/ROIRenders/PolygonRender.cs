using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
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
            get => ROIDesc as PolygonDesc;
            set
            {
                if (ROIDesc != value)
                {
                    ROIDesc = value;
                    OnPropertyChanged();
                }

            }
        }

        public List<HPImageViewer.Core.Primitives.Point> Points
        {
            get => PolygonDesc.Vertices;
            set
            {
                if (PolygonDesc.Vertices != value)
                {
                    PolygonDesc.Vertices = value;
                    OnPropertyChanged();
                }

            }
        }

        public bool IsClosed
        {
            get;
            set;
        } = true;
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
                var drawingContext = renderContext.DrawingContext;
                //for (int i = 0; i < Points.Count - 1; i++)
                //{

                //    drawingContext.DrawLine(new Pen(Brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(Points[i]), RenderTransform.ToDevice(Points[i + 1]));

                //}

                //if (IsClosed)
                //{
                //    drawingContext.DrawLine(new Pen(Brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(Points[Points.Count - 1]), RenderTransform.ToDevice(Points[0]));
                //}
                var fillBrush = IsSelected ? FillBrush : null;
                if (fillBrush?.IsFrozen == false)
                {
                    fillBrush.Freeze();
                }
                if (IsClosed)
                {
                    drawingContext.DrawPolygon(fillBrush, new Pen(Brush, ROIDesc.StrokeThickness), Points.Select(s => RenderTransform.ToDevice(s)));
                }
                else
                {
                    for (int i = 0; i < Points.Count - 1; i++)
                    {

                        drawingContext.DrawLine(new Pen(Brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(Points[i]).ToWindowPoint().ToPoint(), RenderTransform.ToDevice(Points[i + 1]));
                    }
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
            if (IsPointInPolygon(RenderTransform.ToDomain(point.ToPoint()), PolygonDesc))
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

        protected override void MoveHandleToInternal(int handleNumber, Point point)
        {

            Points[handleNumber - 1] = point.ToPoint();
        }

        public override Point GetHandle(int handleNumber)
        {
            return RenderTransform.ToDevice(Points[handleNumber - 1]).ToWindowPoint();
        }

        public static bool IsPointInPolygon(Core.Primitives.Point point, PolygonDesc polygon)
        {
            int intersectionCount = 0;
            int n = polygon.Vertices.Count;

            for (int i = 0; i < n; i++)
            {
                Core.Primitives.Point p1 = polygon.Vertices[i];
                Core.Primitives.Point p2 = polygon.Vertices[(i + 1) % n];

                if (IsIntersect(point, p1, p2))
                {
                    intersectionCount++;
                }
            }

            return intersectionCount % 2 == 1;
        }

        private static bool IsIntersect(Core.Primitives.Point point, Core.Primitives.Point p1, Core.Primitives.Point p2)
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

        internal override bool IntersectsWith(Rect rect)
        {
            //算法：转换成是否存在一条Polygon的line与Rect的相交的问题，存在则Polygon与Rect相交
            return base.IntersectsWith(rect);
        }
    }
}
