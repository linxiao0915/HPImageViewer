using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal class RotatedRectRender : ROIRender
    {
        public RotatedRectRender(RotatedRectDesc rotatedRectDesc) : base(rotatedRectDesc) { }

        public RotatedRectDesc RotatedRectDesc => ROIDesc as RotatedRectDesc;
        public double Angle
        {
            get => RotatedRectDesc.Angle;
            set
            {
                if (RotatedRectDesc.Angle != value)
                {
                    RotatedRectDesc.Angle = value;
                    OnPropertyChanged();
                }
            }
        }
        public Rect Rectangle => MathUtil.GetNormalizedRectangle(RotatedRectDesc.CenterX - RotatedRectDesc.Width / 2, RotatedRectDesc.CenterY - RotatedRectDesc.Height / 2, RotatedRectDesc.CenterX + RotatedRectDesc.Width / 2, RotatedRectDesc.CenterY + RotatedRectDesc.Height / 2);
        protected override void OnROIRender(RenderContext renderContext)
        {
            var brush = Brush;
            brush.Freeze();
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }
            var centerPoint = new Point(Rectangle.X + Rectangle.Width / 2, Rectangle.Y + Rectangle.Height / 2);
            var transformedCenterPoint = RenderTransform.ToDevice(centerPoint);


            //var width = Rectangle.Width * renderContext.Scale;
            //var height = Rectangle.Height * renderContext.Scale;
            var matrix = Matrix.Identity;
            matrix.RotateAt(-Angle, centerPoint.X, centerPoint.Y);
            var t = new RenderTransform(matrix);
            var hV = new Vector(Rectangle.Width, 0);
            var vV = new Vector(0, Rectangle.Height);

            var transformedWidth = RenderTransform.ToDevice(t.ToDevice(hV)).Length;
            var transformedHeight = RenderTransform.ToDevice(t.ToDevice(vV)).Length;

            var pen = new Pen(brush, RotatedRectDesc.StrokeThickness);
            pen.Freeze();
            var transformRect = new Rect(transformedCenterPoint.X - transformedWidth / 2, transformedCenterPoint.Y - transformedHeight / 2, transformedWidth, transformedHeight);
            renderContext.DrawingContext.DrawRectangle(fillBrush, pen, transformRect, Angle);
            var arrowPoints = new List<Core.Primitives.Point>();
            arrowPoints.Add(new Core.Primitives.Point(transformRect.Right, transformRect.Top + transformRect.Height / 2));
            arrowPoints.Add(new Core.Primitives.Point(transformRect.Right + 12, transformRect.Top + transformRect.Height / 2));
            arrowPoints.Add(new Core.Primitives.Point(transformRect.Right + 5, transformRect.Top + transformRect.Height / 2 + 5));
            arrowPoints.Add(new Core.Primitives.Point(transformRect.Right + 5, transformRect.Top + transformRect.Height / 2 - 5));

            matrix = Matrix.Identity;
            matrix.RotateAt(-Angle, transformedCenterPoint.X, transformedCenterPoint.Y);
            t = new RenderTransform(matrix);
            arrowPoints = arrowPoints.Select(s => t.ToDevice(s.ToWindowPoint()).ToPoint()).ToList();
            renderContext.DrawingContext.DrawLine(pen, arrowPoints[0], arrowPoints[1]);
            renderContext.DrawingContext.DrawLine(pen, arrowPoints[1], arrowPoints[2]);
            renderContext.DrawingContext.DrawLine(pen, arrowPoints[1], arrowPoints[3]);

            //     renderContext.DrawingContext.DrawLine(pen,);

        }

        public override int HitTest(Point point)
        {
            return 0;
        }

        public override int HandleCount { get; }
        protected override void MoveToInternal(double wx, double wy)
        {
        }

        protected override bool NeedRender(RenderContext renderContext)
        {
            return base.NeedRender(renderContext);
        }
    }
}

