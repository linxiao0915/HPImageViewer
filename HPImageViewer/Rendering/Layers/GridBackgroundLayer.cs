using HPImageViewer.Utils;
using System;
using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Rendering.Layers
{
    internal class GridLayer : RenderBase, ILayer
    {
        static Pen _LightPen;
        static Pen _DarkPen;


        static GridLayer()
        {
            _LightPen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3C3C3C")), 0);
            _LightPen.Freeze();
            _DarkPen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D")), 0);
            _DarkPen.Freeze();
        }


        public int HorizontalInternal { get; set; } = 20;
        public int VerticalInternal { get; set; } = 20;


        protected override void OnRender(RenderContext renderContext)
        {
            var renderWidth = (int)renderContext.RenderSize.Width;
            var renderHeight = (int)renderContext.RenderSize.Height;

            if (renderWidth <= 0 || renderHeight <= 0)
                return;

            var xStart = 0;// (renderWidth - HorizontalInternal * (renderWidth / HorizontalInternal)) / 2 - HorizontalInternal;
            var yStart = 0;// (renderHeight - VerticalInternal * (renderHeight / VerticalInternal)) / 2 - VerticalInternal;

            renderContext.RenderTransform.ToDevice(xStart, yStart, out var deviceXStart, out var deviceYStart);

            var deviceHorizontalInternal = renderContext.Scale * HorizontalInternal;
            var deviceVerticalInternal = renderContext.Scale * VerticalInternal;
            var drawingContext = renderContext.DrawingContext;
            if (deviceVerticalInternal < 10 || deviceHorizontalInternal < 10 || deviceHorizontalInternal > renderWidth || deviceVerticalInternal > renderHeight)
            {
                drawingContext.DrawRectangle(_DarkPen.Brush, _DarkPen, new Rect(0, 0, renderContext.RenderSize.Width, renderContext.RenderSize.Height));
                return;
            }

            var xCount = Math.Abs(deviceXStart / (deviceHorizontalInternal * 2));

            if (deviceXStart <= 0)
            {
                deviceXStart += ((int)xCount) * deviceHorizontalInternal * 2;
            }
            else
            {
                deviceXStart -= ((int)xCount + 1) * deviceHorizontalInternal * 2;
            }


            var yCount = Math.Abs(deviceYStart / (deviceVerticalInternal * 2));

            if (deviceYStart <= 0)
            {

                deviceYStart += ((int)yCount) * deviceVerticalInternal * 2;
            }
            else
            {
                deviceYStart -= ((int)yCount + 1) * deviceVerticalInternal * 2;
            }


            var yIndex = 0;
            var yPosition = deviceYStart;
            do
            {
                var xPosition = deviceXStart;
                var xIndex = 0;
                var pen = yIndex++ % 2 == 0 ? _LightPen : _DarkPen;

                do
                {
                    var pen2 = xIndex++ % 2 == 0 ? pen : pen == _LightPen ? _DarkPen : _LightPen;
                    var rect = new Rect(xPosition, yPosition, (int)deviceHorizontalInternal + 1, (int)deviceVerticalInternal + 1);



                    drawingContext.DrawRectangle(pen2.Brush, pen2, rect);

                    xPosition += (int)deviceHorizontalInternal;
                } while (xPosition <= renderWidth);

                yPosition += (int)deviceVerticalInternal;
            } while (yPosition <= renderHeight);

        }

    }
}
