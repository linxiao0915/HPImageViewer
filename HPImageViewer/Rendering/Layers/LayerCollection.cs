using System.Collections.Generic;

namespace HPImageViewer.Rendering.Layers
{
    internal class LayerCollection
    {
        public List<ILayer> BackgroundLayers { get; set; } = new List<ILayer>()
        {
            // new GridLayer(),
            //new CrossHairLayer(),
            //new ViewingInfoLayer(),
        };
        public List<ILayer> ForegroundLayers { get; set; } = new List<ILayer>()
            {
                //new GridBackgroundLayer(),
                new CrossHairLayer(),
               // new ViewingInfoLayer(),
            };

        public LayerCollection()
        {

        }




    }
}
