using System.Collections.Generic;
using System.Linq;

namespace HPImageViewer.Rendering.ROIRenders
{
    internal static class RenderExtensions
    {

        public static void UnselectAll(this IEnumerable<ROIRender> renders)
        {
            foreach (var render in renders)
            {
                render.IsSelected = false;
            }
        }

        public static void SelectAll(this IEnumerable<ROIRender> renders)
        {
            foreach (var render in renders)
            {
                render.IsSelected = true;
            }
        }

        public static IList<ROIRender> GetSelectedItems(this IEnumerable<ROIRender> renders)
        {
            return renders.Where(n => n.IsSelected).ToList();
        }
    }
}
