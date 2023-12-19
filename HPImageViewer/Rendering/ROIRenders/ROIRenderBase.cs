﻿using HPImageViewer.Core.Persistence;
using System.Windows;
using System.Windows.Media;


namespace HPImageViewer.Rendering.ROIRenders
{
    internal abstract class ROIRender : RenderBase, IMovable, IResizable
    {
        public ROIDesc ROIDesc { get; protected set; }

        protected ROIRender(ROIDesc roiDesc)
        {
            ROIDesc = roiDesc;
        }
        //todo: render不应存在brushUI元素
        SolidColorBrush _brush;
        protected SolidColorBrush Brush
        {
            get
            {
                if (_brush == null)
                {
                    _brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Color));
                }
                return _brush;
            }
        }
        public string Color
        {
            get => ROIDesc.Color;
            set
            {
                ROIDesc.Color = value;
                _brush = null;
            }
        }

        IDrawingCanvas DrawingCanvas { get; }

        protected sealed override void OnRender(RenderContext renderContext)
        {
            if (IsSelected)
            {
                DrawTracker(renderContext);
            }
            OnROIRender(renderContext);
        }

        protected abstract void OnROIRender(RenderContext renderContext);

        public abstract int HitTest(Point point);

        public bool IsSelected { get; set; }

        /// <summary>
        /// Draw tracker for selected object
        /// </summary>
        /// <param name="g"></param>
        private void DrawTracker(RenderContext renderContext)
        {
            if (!IsSelected)
                return;
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ROIDesc.Color));
            brush.Freeze();
            for (int i = 1; i <= HandleCount; i++)
            {
                var rect = GetHandlePoint(i);
                var drawingContext = renderContext.DrawingContext;
                drawingContext.DrawRectangle(brush, new Pen(brush, ROIDesc.StrokeThickness), rect);
            }

        }

        #region IResizable'S Implementations

        public void MoveHandleTo(int handleNumber, Point point)
        {
            var p = RenderTransform.ToDomain(point);
            MoveHandleToInteranl(handleNumber, p);
        }

        protected virtual void MoveHandleToInteranl(int handleNumber, Point point)
        {

        }

        public virtual Point GetHandle(int handleNumber)
        {
            return new Point(0, 0);
        }

        public Rect GetHandlePoint(int handleNumber)
        {
            var point = GetHandle(handleNumber);
            return new Rect(point.X - 2, point.Y - 2, 5, 5);
        }

        public abstract int HandleCount { get; }

        #endregion

        public void MoveTo(double x, double y)
        {
            var v = RenderTransform.ToDomain(new Vector(x, y));
            MoveToInternal(v.X, v.Y);
        }

        protected abstract void MoveToInternal(double wx, double wy);

        public virtual void Normalize()
        {

        }

        internal virtual bool IntersectsWith(Rect rect)
        {
            return false;

        }
    }




}
