using System.Windows;

namespace HPImageViewer.Rendering
{
    internal interface IResizable
    {
        public void MoveHandleTo(int handleNumber, Point point);
        public Point GetHandle(int handleNumber);
        //public Rect GetHandlePoint(int handleNumber);
        public int HandleCount { get; }
    }
}
