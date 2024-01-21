using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal class SelectionRectangle : RectangleRender
    {
        protected override void OnROIRender(RenderContext renderContext)
        {
            var drawingContext = renderContext.DrawingContext;
            drawingContext.DrawRegularRectangle(
            null,
                 new Pen(Brushes.White, ROIDesc.StrokeThickness),
                 DeviceRectangle);

            DashStyle dashStyle = new DashStyle();
            dashStyle.Dashes.Add(4);

            Pen dashedPen = new Pen(Brushes.Black, ROIDesc.StrokeThickness);
            dashedPen.DashStyle = dashStyle;


            drawingContext.DrawRegularRectangle(
                null,
                dashedPen,
                DeviceRectangle);
        }

    }
}
