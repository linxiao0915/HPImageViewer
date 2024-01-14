using HPImageViewer.Utils;

namespace HPImageViewer.Rendering
{
    internal abstract class RenderBase : IRender
    {
        public ICoordTransform RenderTransform { get; set; }
        public void Render(RenderContext renderContext)
        {
            RenderTransform = renderContext.RenderTransform;
            if (renderContext.RenderSize.Width <= 0
                || renderContext.RenderSize.Height <= 0
                || NeedRender(renderContext) == false)
                return;
            OnRender(renderContext);
        }

        protected abstract void OnRender(RenderContext renderContext);


        protected virtual bool NeedRender(RenderContext renderContext)
        {
            return true;
        }
    }
}
