
using System;
using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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
            UpdateFromDesc();
        }
        private double _left;
        public double Left
        {
            get => _left;
            set
            {
                if (_left != value)
                {
                    _left = value;
                    UpdateToDesc();
                    OnPropertyChanged();
                }

            }
        }
        private double _top;
        public double Top
        {
            get => _top;
            set
            {
                if (_top != value)
                {
                    _top = value;
                    UpdateToDesc();
                    OnPropertyChanged();
                }

            }
        }
        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    UpdateToDesc();
                    OnPropertyChanged();
                }

            }
        }
        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    UpdateToDesc();
                    OnPropertyChanged();
                }

            }
        }
        private void UpdateToDesc()
        {
            RectangleDesc.Left = Rect.Left;
            RectangleDesc.Top = Rect.Top;
            RectangleDesc.Width = Rect.Width;
            RectangleDesc.Height = Rect.Height;
        }
        private void UpdateFromDesc()
        {
            _left = RectangleDesc.Left;
            _top = RectangleDesc.Top;
            _width = RectangleDesc.Width;
            _height = RectangleDesc.Height;
        }

        public Core.Primitives.Rect Rect => MathUtil.GetNormalizedRectangle(Left, Top, Left + Width, Top + Height);

        public virtual Core.Primitives.Rect Bound => this.Rect;

        protected override void OnROIRender(RenderContext renderContext)
        {
            var brush = Brush;
            brush.Freeze();
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }
            var originalPoint = new Core.Primitives.Point(Rect.X, Rect.Y);
            var transformedPoint = RenderTransform.ToDevice(originalPoint);
            var width = Rect.Width * renderContext.Scale;
            var height = Rect.Height * renderContext.Scale;

            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, RectangleDesc.StrokeThickness), new Core.Primitives.Rect(transformedPoint.X, transformedPoint.Y, width, height));
        }

        protected Core.Primitives.Rect DeviceRect => RenderTransform.ToDevice(Rect);

        protected Core.Primitives.Rect DeviceBound => RenderTransform.ToDevice(Bound);

        public override int HitTest(Point point)
        {
            UpdateFromDesc();
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
            return DeviceBound.Contains(devicePoint.ToPoint());
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            return RenderTransform.ToDevice(GetRectHandle(Rect, handleNumber)).ToWindowPoint();
        }

        internal static Core.Primitives.Point GetRectHandle(Core.Primitives.Rect rectangle, int handleNumber, bool includeCenterPoint = true)
        {
            double x, y, xCenter, yCenter;
            xCenter = rectangle.X + rectangle.Width / 2;
            yCenter = rectangle.Y + rectangle.Height / 2;
            x = rectangle.X;
            y = rectangle.Y;

            if (includeCenterPoint == false)
            {
                handleNumber = (handleNumber - 1) * 2 + 1;
            }

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
            return new Core.Primitives.Point(x, y);
        }

        public override int HandleCount => 8;

        protected override void MoveHandleToInternal(int handleNumber, Point point)
        {

            var rect = MoveRectangle(handleNumber, point, Left, Top, Width, Height);
            Left = rect.l;
            Top = rect.t;
            Width = rect.w;
            Height = rect.h;
        }
        protected static (double l, double t, double w, double h) MoveRectangle(int handleNumber, Point point, double left, double top, double width, double height, bool includeCenterPoint = true)
        {
            var x = point.X;
            var y = point.Y;

            //var left = rect.Left;
            //var top = rect.Top;
            var right = left + width;
            var bottom = top + height;

            if (includeCenterPoint == false)
            {
                handleNumber = (handleNumber - 1) * 2 + 1;
            }
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

            width = right - left;
            height = bottom - top;
            return (left, top, width, height);
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
            return NeedRender(new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height), DeviceRect.ToWindowRect(), RectangleDesc);

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
            return Rect.IntersectsWith(rect.ToRect());
        }
    }
}
