﻿
using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{

    internal class RectangleRender : ROIRender
    {

        private RectangleDesc rectangleDesc
        {
            get => (RectangleDesc)_rOIDesc;
            set => _rOIDesc = value;
        }

        public RectangleRender() : this(new RectangleDesc())
        {

        }
        public RectangleRender(RectangleDesc rectangleDesc) : base(rectangleDesc)
        {
        }

        public double Left
        {
            get => rectangleDesc.Left;
            set => rectangleDesc.Left = value;
        }
        public double Top
        {
            get => rectangleDesc.Top;
            set => rectangleDesc.Top = value;
        }

        public double Width
        {
            get => rectangleDesc.Width;
            set => rectangleDesc.Width = value;
        }

        public double Height
        {
            get => rectangleDesc.Height;
            set => rectangleDesc.Height = value;
        }

        public Rect Rectangle => RectUtil.GetNormalizedRectangle(rectangleDesc.Left, rectangleDesc.Top, rectangleDesc.Left + rectangleDesc.Width, rectangleDesc.Top + rectangleDesc.Height);


        protected override void OnRender(RenderContext renderContext)
        {
            base.OnRender(renderContext);
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(rectangleDesc.Color));
            brush.Freeze();
            var originalPoint = new Point(Rectangle.X, Rectangle.Y);
            var transformedPoint = RenderTransform.ToDevice(originalPoint);
            var width = Rectangle.Width * renderContext.Scale;
            var height = Rectangle.Height * renderContext.Scale;
            renderContext.DrawRectangle(Brushes.Transparent, new Pen(brush, rectangleDesc.StrokeThickness), new Rect(transformedPoint.X, transformedPoint.Y, width, height));
        }

        private Rect DeviceRectangle => RenderTransform.ToDevice(Rectangle);

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
            double x, y, xCenter, yCenter;

            var rectangle = Rectangle;

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

            return RenderTransform.ToDevice(new Point(x, y));

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
            var deviceRenderOuterBound = RectUtil.GetOuterBoundRectangle(DeviceRectangle, rectangleDesc.StrokeThickness);

            var deviceRenderInnerBound = RectUtil.GetInnerBoundRectangle(DeviceRectangle, rectangleDesc.StrokeThickness);
            var renderRect = new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height);
            var innerBoundWidth = deviceRenderInnerBound.Width;
            var innerBoundHeight = deviceRenderInnerBound.Height;
            if (innerBoundWidth <= 0 || innerBoundHeight <= 0)
            {
                return renderRect.IntersectsWith(deviceRenderOuterBound);
            }

            return renderRect.IntersectsWith(deviceRenderOuterBound) && deviceRenderInnerBound.Contains(renderRect) == false;

        }
    }
}