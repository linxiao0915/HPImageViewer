using HPImageViewer.Core.Persistence;
using HPImageViewer.Rendering.ROIRenders;

namespace HPImageViewer.Utils
{
    internal static class RenderFactory
    {

        public static ROIRender CreateROIRender(ROIDesc rOIDesc)
        {
            dynamic rOI = rOIDesc;
            return CreateROIRenderInternal(rOI);
        }

        private static RectangleRender CreateROIRenderInternal(RectangleDesc rectangleDesc)
        {
            return new RectangleRender(rectangleDesc);
        }

        private static EllipseRender CreateROIRenderInternal(EllipseDesc ellipseDesc)
        {
            return new EllipseRender(ellipseDesc);
        }


        private static PolygonRender CreateROIRenderInternal(PolygonDesc polygonDesc)
        {
            return new PolygonRender(polygonDesc);
        }
    }
}
