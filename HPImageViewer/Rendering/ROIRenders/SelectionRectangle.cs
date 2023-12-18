﻿using System.Windows.Media;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal class SelectionRectangle : RectangleRender
    {
        protected override void OnROIRender(RenderContext renderContext)
        {
            renderContext.DrawRectangle(
            null,
                 new Pen(Brushes.White, _rOIDesc.StrokeThickness),
                 DeviceRectangle);

            DashStyle dashStyle = new DashStyle();
            dashStyle.Dashes.Add(4);

            Pen dashedPen = new Pen(Brushes.Black, _rOIDesc.StrokeThickness);
            dashedPen.DashStyle = dashStyle;


            renderContext.DrawRectangle(
                null,
                dashedPen,
                DeviceRectangle);
        }

    }
}
