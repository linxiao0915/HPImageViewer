using HPImageViewer.Core.Persistence;
using HPImageViewer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using HPImageViewer.Core.Primitives;
using Point = System.Windows.Point;
using System.Windows;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal class BoxRender : RectangleRender
    {

        public BoxRender() : this(new BoxDesc())
        {
        }
        private BoxDesc BoxDesc => (BoxDesc)ROIDesc;
        public BoxRender(BoxDesc boxDesc) : base(boxDesc)
        {
        }
        public override Core.Primitives.Rect Bound => MathUtil.GetNormalizedRectangle(Left - BandWidth / 2, Top - BandWidth / 2, Left + Width + BandWidth / 2, Top + Height + BandWidth / 2);
        public Core.Primitives.Rect InnerBound
        {
            get
            {

                if (Bound.Width <= BandWidth || Bound.Height <= BandWidth)
                {
                    return new Core.Primitives.Rect();
                }

                var left = Rect.Left + BandWidth / 2;
                var top = Rect.Top + BandWidth / 2;
                var right = Rect.Right - BandWidth / 2;
                var bottom = Rect.Bottom - BandWidth / 2;

                return MathUtil.GetNormalizedRectangle(left, top, right, bottom);
            }
        }

        public double BandWidth
        {
            get => BoxDesc.BandWidth;
            set
            {
                if (BoxDesc.BandWidth != value)
                {
                    BoxDesc.BandWidth = value;
                    OnPropertyChanged();
                }

            }
        }

        protected override void OnROIRender(RenderContext renderContext)
        {
            var brush = Brush;
            brush.Freeze();
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }

            renderContext.DrawingContext.DrawRegularRectangle(Brushes.Transparent, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(Bound));
            renderContext.DrawingContext.DrawRegularRectangle(Brushes.Transparent, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(InnerBound));
            var deviceBandWidth = BandWidth * renderContext.Scale;
            renderContext.DrawingContext.DrawRegularRectangle(Brushes.Transparent, new Pen(FillBrush, deviceBandWidth), RenderTransform.ToDevice(Rect));
            //renderContext.DrawingContext.DrawRegularRectangle(Brushes.Transparent, new Pen(Brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(Rect));
        }

        public override Point GetHandle(int handleNumber)
        {
            var index = (handleNumber - 1) / 8;
            var remainder = (handleNumber - 1) % 8 + 1;

            if (index == 0)
            {
                return RenderTransform.ToDevice(GetRectHandle(Rect, remainder)).ToWindowPoint();
            }
            else if (index == 1)
            {
                var subIndex = (remainder - 1) / 4;
                var subReminder = (remainder - 1) % 4 + 1;
                if (subIndex == 0)
                {
                    return RenderTransform.ToDevice(GetRectHandle(Bound, subReminder, false)).ToWindowPoint();
                }
                else if (subIndex == 1)
                {
                    return RenderTransform.ToDevice(GetRectHandle(InnerBound, subReminder, false)).ToWindowPoint();
                }

            }
            throw new ArgumentOutOfRangeException($"无效的{nameof(handleNumber)}");
        }

        public override int HandleCount => 16;
        protected override void MoveHandleToInternal(int handleNumber, Point point)
        {
            var index = (handleNumber - 1) / 8;
            var remainder = (handleNumber - 1) % 8 + 1;
            if (index == 0)
            {
                base.MoveHandleToInternal(handleNumber, point);
            }
            else if (index == 1)
            {
                var subIndex = (remainder - 1) / 4;
                var subReminder = (remainder - 1) % 4 + 1;
                if (subIndex == 0)
                {

                    var (l, t, w, h) = MoveRectangle(subReminder, point, Bound.Left, Bound.Top, Bound.Width, Bound.Height, false);
                    var rect = MathUtil.GetNormalizedRectangle(l, t, l + w, t + h);
                    if (rect.Width > Rect.Width && rect.Width < Rect.Width * 2)
                    {
                        BandWidth = rect.Width - Rect.Width;
                    }
                }
                else if (subIndex == 1)
                {
                    var (l, t, w, h) = MoveRectangle(subReminder, point, InnerBound.Left, InnerBound.Top, InnerBound.Width, InnerBound.Height, false);
                    var rect = MathUtil.GetNormalizedRectangle(l, t, l + w, t + h);
                    if (Rect.Width > rect.Width)
                    {
                        BandWidth = Rect.Width - rect.Width;
                    }

                }
            }

        }
    }
}
