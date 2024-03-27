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

namespace HPImageViewer.Rendering.ROIRenders
{

    internal class QuadRectangleRender : BoxRender
    {
        public QuadRectangleRender(QuadRectangleDesc quadRectangleDesc) : base(quadRectangleDesc)
        {

        }

        protected override void OnROIRender(RenderContext renderContext)
        {
            //  base.OnROIRender(renderContext);

            var brush = Brush;
            brush.Freeze();
            var fillBrush = IsSelected ? FillBrush : null;
            if (fillBrush?.IsFrozen == false)
            {
                fillBrush.Freeze();
            }

            renderContext.DrawingContext.DrawRegularRectangle(Brushes.Transparent, new Pen(FillBrush, ROIDesc.StrokeThickness)
            {
                DashStyle = new DashStyle(new double[] { 4, 4 }, 0)
            }, RenderTransform.ToDevice(Rect));

            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(LeftRect));
            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(TopRect));
            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(RightRect));
            renderContext.DrawingContext.DrawRegularRectangle(fillBrush, new Pen(brush, ROIDesc.StrokeThickness), RenderTransform.ToDevice(BottomRect));
        }

        public QuadRectangleDesc QuadRectangleDesc => this.ROIDesc as QuadRectangleDesc;
        public double BandLength
        {
            get => QuadRectangleDesc.BandLength;
            set
            {
                if (QuadRectangleDesc.BandLength != value)
                {
                    QuadRectangleDesc.BandLength = value;
                    OnPropertyChanged();
                }

            }
        }

        public Rect LeftRect
        {
            get
            {
                var bound = Bound;
                var left = bound.Left;
                var top = bound.Top + bound.Height / 2 - BandLength / 2;
                return new Rect(left, top, BandWidth, BandLength);
            }
        }
        public Rect TopRect
        {

            get
            {
                var bound = Bound;
                var left = bound.Left + bound.Width / 2 - BandLength / 2;
                var top = bound.Top;
                return new Rect(left, top, BandLength, BandWidth);

            }
        }
        public Rect RightRect
        {

            get
            {
                var leftTopRect = LeftRect;
                var left = leftTopRect.Left + Rect.Width;
                var top = leftTopRect.Top;
                return new Rect(left, top, leftTopRect.Width, leftTopRect.Height);

            }
        }
        public Rect BottomRect
        {

            get
            {
                var topRect = TopRect;
                var left = topRect.Left;
                var top = Bound.Top + Rect.Height;
                return new Rect(left, top, topRect.Width, topRect.Height);

            }
        }


        /// <summary>Get handle point by 1-based number</summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
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
                return RenderTransform.ToDevice(GetRectHandle(LeftRect, remainder)).ToWindowPoint();
            }
            else if (index == 2)
            {
                return RenderTransform.ToDevice(GetRectHandle(TopRect, remainder)).ToWindowPoint();
            }
            else if (index == 3)
            {
                return RenderTransform.ToDevice(GetRectHandle(RightRect, remainder)).ToWindowPoint();
            }
            else if (index == 4)
            {
                return RenderTransform.ToDevice(GetRectHandle(BottomRect, remainder)).ToWindowPoint();
            }

            throw new ArgumentOutOfRangeException($"无效的{nameof(handleNumber)}");
        }

        public override int HandleCount => 40;


        protected override void MoveHandleToInternal(int handleNumber, Point point)
        {
            base.MoveHandleToInternal(handleNumber, point);
            var index = (handleNumber - 1) / 8;
            var remainder = (handleNumber - 1) % 8 + 1;
            if (index == 0)
            {
                base.MoveHandleToInternal(handleNumber, point);
                ////var (left, top, right, bottom) = MoveRectangle(remainder, point, Rect);
                //Left = left;
                //Top = top;
                //Width = right - left;
                //Height = bottom - top;

            }
            else
            {
                Rect rect;
                if (index == 1)
                {
                    rect = LeftRect;
                }
                else if (index == 2)
                {
                    rect = TopRect;
                }
                else if (index == 3)
                {
                    rect = RightRect;
                }
                else if (index == 4)
                {
                    rect = BottomRect;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"无效的{nameof(index)}");
                }

                var (l, t, w, h) = MoveRectangle(remainder, point, rect.Left, rect.Top, rect.Width, rect.Height);
                var normalizedRect = MathUtil.GetNormalizedRectangle(l, t, l + w, t + h);

                if (index == 1 || index == 3)
                {
                    BandWidth = normalizedRect.Width;
                    BandLength = normalizedRect.Height;
                }
                else
                {
                    BandWidth = normalizedRect.Height;
                    BandLength = normalizedRect.Width;
                }
            }

        }
    }


}
