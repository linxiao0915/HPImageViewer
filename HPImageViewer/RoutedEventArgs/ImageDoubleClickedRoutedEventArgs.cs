using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Size = HPImageViewer.Core.Primitives.Size;

namespace HPImageViewer.RoutedEventArgs
{
    public class ImageDoubleClickedRoutedEventArgs : System.Windows.RoutedEventArgs
    {
        public ImageDoubleClickedRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
        {

        }

        public Point Position { get; internal set; }
        public Size ImageSize { get; internal set; }
    }
}
