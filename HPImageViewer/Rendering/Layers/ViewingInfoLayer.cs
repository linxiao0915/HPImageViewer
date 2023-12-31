﻿using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.Layers
{
    internal class ViewingInfoLayer : IBackgroundLayer
    {
        public void Render(RenderContext renderContext)
        {
            var drawingContext = renderContext.DrawingContext;
            Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var formattedText = new FormattedText($"{(renderContext.Scale * 100).ToString("F2")}%", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 10, Brushes.White, 96.0);
            drawingContext.DrawText(formattedText, new Point(10, 10));

            //var formattedText2 = new FormattedText($"图片计数：{Count}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, 80, Brushes.White, 96.0);
            //drawingContext.DrawText(formattedText2, new Point(80, 100));

        }

        //public int Count { get; set; } = 0;
    }
}
