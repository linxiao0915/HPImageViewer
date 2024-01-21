﻿
using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{

    internal class RectangleRender : ROIRender
    {

        private RectangleDesc RectangleDesc => (RectangleDesc)ROIDesc;

        public RectangleRender() : this(new RectangleDesc())
        {

        }
        public RectangleRender(RectangleDesc rectangleDesc) : base(rectangleDesc)
        {
        }

        public double Left
        {
            get => RectangleDesc.Left;
            set
            {
                if (RectangleDesc.Left != value)
                {
                    RectangleDesc.Left = value;
                    OnPropertyChanged();
                }

            }
        }

        public double Top
        {
            get => RectangleDesc.Top;
            set
            {
                if (RectangleDesc.Top != value)
                {
                    RectangleDesc.Top = value;
                    OnPropertyChanged();
                }

            }
        }

        public double Width
        {
            get => RectangleDesc.Width;
            set
            {
                if (RectangleDesc.Width != value)
                {
                    RectangleDesc.Width = value;
                    OnPropertyChanged();
                }

            }
        }

        public double Height
        {
            get => RectangleDesc.Height;
            set
            {
                if (RectangleDesc.Height != value)
                {
                    RectangleDesc.Height = value;
                    OnPropertyChanged();
                }

            }
        }

        public Rect Rectangle => MathUtil.GetNormalizedRectangle(RectangleDesc.Left, RectangleDesc.Top, RectangleDesc.Left + RectangleDesc.Width, RectangleDesc.Top + RectangleDesc.Height);


        protected override void OnROIRender(RenderContext renderContext)
        {
            var brush = Brush;
            brush.Freeze();
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }
            var originalPoint = new Point(Rectangle.X, Rectangle.Y);
            var transformedPoint = RenderTransform.ToDevice(originalPoint);
            var width = Rectangle.Width * renderContext.Scale;
            var height = Rectangle.Height * renderContext.Scale;

            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, RectangleDesc.StrokeThickness), new Rect(transformedPoint.X, transformedPoint.Y, width, height));
        }

        protected Rect DeviceRectangle => RenderTransform.ToDevice(Rectangle);

        public override int HitTest(Point point)
        {
            if (IsSelected)
            {
                for (int i = 1; i <= HandleCount; i++)
                {
                    if (GetHandlePoint(i).Contains(point))
                        return i;
                }
            }

            if (PointInObject(point))
                return 0;

            return -1;
        }

        protected bool PointInObject(Point devicePoint)
        {
            return DeviceRectangle.Contains(devicePoint);
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            return RenderTransform.ToDevice(GetRectHandle(Rectangle, handleNumber));
        }

        internal static Point GetRectHandle(Rect rectangle, int handleNumber)
        {
            double x, y, xCenter, yCenter;
            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            switch (handleNumber)
            {
                case 1:
                    x = rectangle.X;
                    y = rectangle.Y;
                    break;
                case 2:
                    x = xCenter;
                    y = rectangle.Y;
                    break;
                case 3:
                    x = rectangle.Right;
                    y = rectangle.Y;
                    break;
                case 4:
                    x = rectangle.Right;
                    y = yCenter;
                    break;
                case 5:
                    x = rectangle.Right;
                    y = rectangle.Bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = rectangle.Bottom;
                    break;
                case 7:
                    x = rectangle.X;
                    y = rectangle.Bottom;
                    break;
                case 8:
                    x = rectangle.X;
                    y = yCenter;
                    break;
            }
            return new Point(x, y);
        }

        public override int HandleCount => 8;

        protected override void MoveHandleToInteranl(int handleNumber, Point point)
        {
            var x = point.X;
            var y = point.Y;

            var left = Left;
            var top = Top;
            var right = Left + Width;
            var bottom = Top + Height;

            switch (handleNumber)
            {
                case 1:
                    left = x;
                    top = y;
                    break;
                case 2:
                    top = y;
                    break;
                case 3:
                    right = x;
                    top = y;
                    break;
                case 4:
                    right = x;
                    break;
                case 5:
                    right = x;
                    bottom = y;
                    break;
                case 6:
                    bottom = y;
                    break;
                case 7:
                    left = x;
                    bottom = y;
                    break;
                case 8:
                    left = x;
                    break;
            }

            Left = left;
            Top = top;
            Width = right - left;
            Height = bottom - top;
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        protected override void MoveToInternal(double wx, double wy)
        {
            Left += wx;
            Top += wy;
        }



        protected override bool NeedRender(RenderContext renderContext)
        {
            return NeedRender(new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height), DeviceRectangle, RectangleDesc);

        }

        internal static bool NeedRender(Rect deviceArea, Rect roiDeviceRect, ROIDesc roiDesc)
        {

            var deviceRenderOuterBound = MathUtil.GetOuterBoundRectangle(roiDeviceRect, roiDesc.StrokeThickness);

            var deviceRenderInnerBound = MathUtil.GetInnerBoundRectangle(roiDeviceRect, roiDesc.StrokeThickness);

            var innerBoundWidth = deviceRenderInnerBound.Width;
            var innerBoundHeight = deviceRenderInnerBound.Height;
            if (innerBoundWidth <= 0 || innerBoundHeight <= 0)
            {
                return deviceArea.IntersectsWith(deviceRenderOuterBound);
            }

            return deviceArea.IntersectsWith(deviceRenderOuterBound) && deviceRenderInnerBound.Contains(deviceArea) == false;
        }

        internal override bool IntersectsWith(Rect rect)
        {
            return Rectangle.IntersectsWith(rect);
        }
    }
}
