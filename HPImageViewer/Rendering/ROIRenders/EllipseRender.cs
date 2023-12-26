using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal class EllipseRender : ROIRender
    {
        public EllipseRender() : this(new EllipseDesc())
        {

        }


        public EllipseRender(EllipseDesc ellipseDesc) : base(ellipseDesc)
        {
        }

        private EllipseDesc EllipseDesc
        {
            get => (EllipseDesc)ROIDesc;
            set => ROIDesc = value;
        }

        public double CenterX
        {
            get => EllipseDesc.CenterX;
            set => EllipseDesc.CenterX = value;
        }
        public double CenterY
        {
            get => EllipseDesc.CenterY;
            set => EllipseDesc.CenterY = value;
        }

        public double R
        {
            get => EllipseDesc.R;
            set => EllipseDesc.R = value;
        }

        public Rect Rectangle => new Rect(CenterX - Math.Abs(R), CenterY - Math.Abs(R), 2 * Math.Abs(R), 2 * Math.Abs(R));
        private Rect DeviceRectangle => RenderTransform.ToDevice(Rectangle);
        protected bool PointInObject(Point devicePoint)
        {
            return DeviceRectangle.Contains(devicePoint);
        }

        public override int HitTest(Point point)
        {
            if (MathUtil.IsPointInCircle(RenderTransform.ToDomain(point), new Point(CenterX, CenterY), R - ROIDesc.StrokeThickness / 2))
            {
                return 0;
            }

            if (IsSelected)
            {

                if (MathUtil.IsPointInCircle(RenderTransform.ToDomain(point), new Point(CenterX, CenterY), R + ROIDesc.StrokeThickness / 2))
                {
                    return 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            var rectangle = Rectangle;
            double x, y, xCenter, yCenter;
            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = xCenter;
                    y = rectangle.Y;
                    break;
                case 2:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 3:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 4:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }
            return RenderTransform.ToDevice(new Point(x, y));
        }

        public override int HandleCount => 4;

        protected override void MoveToInternal(double wx, double wy)
        {
            CenterX += wx;
            CenterY += wy;
        }

        protected override bool NeedRender(RenderContext renderContext)
        {
            return RectangleRender.NeedRender(new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height), DeviceRectangle, EllipseDesc);
        }

        protected override void OnROIRender(RenderContext renderContext)
        {
            if (Brush.IsFrozen == false)
            {
                Brush.Freeze();
            }
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }


            var center = new Point(CenterX, CenterY);
            var transformedCenter = RenderTransform.ToDevice(center);

            renderContext.DrawingContext.DrawEllipse(fillBrush, new Pen(Brush, this.ROIDesc.StrokeThickness), transformedCenter, R * renderContext.Scale, R * renderContext.Scale);
        }

        protected override void MoveHandleToInteranl(int handleNumber, Point point)
        {
            var x = point.X;
            var y = point.Y;
            var centerVector = new Vector(CenterX, CenterY);
            var currentVector = new Vector(x, y);
            R = (centerVector - currentVector).Length;
        }

        internal override bool IntersectsWith(Rect rect)
        {
            return MathUtil.CircleIntersectsRectangle(CenterX, CenterY, R + ROIDesc.StrokeThickness / 2, rect);
        }


    }
}
