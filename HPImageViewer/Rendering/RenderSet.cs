namespace HPImageViewer.Rendering
{
    internal class RenderSet
    {
        public RenderContext RenderContext { get; private set; }

        public RenderSet(RenderContext renderContext)
        {
            RenderContext = renderContext;
        }
        public ImageRender ImageRender { get; set; }
    }
}
