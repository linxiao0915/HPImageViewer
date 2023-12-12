using HPImageViewer.Core.Persistence;
using System.Windows;
using System.Windows.Media;


namespace HPImageViewer.Rendering.ROIRenders
{
    internal abstract class ROIRender : RenderBase, IMovable, IResizable
    {
        protected ROIDesc _rOIDesc;

        protected ROIRender(ROIDesc rOIDesc)
        {
            _rOIDesc = rOIDesc;
        }

        IDrawingCanvas DrawingCanvas { get; }

        protected override void OnRender(RenderContext renderContext)
        {
            ;
            if (IsSelected)
            {
                DrawTracker(renderContext);
            }
        }

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
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_rOIDesc.Color));
            brush.Freeze();
            for (int i = 1; i <= HandleCount; i++)
            {
                var rect = GetHandlePoint(i);
                var drawingContext = renderContext.DrawingContext;
                drawingContext.DrawRectangle(brush, new Pen(brush, _rOIDesc.StrokeThickness), rect);
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
            return new Rect(point.X - 3, point.Y - 3, 7, 7);
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

    }




}
