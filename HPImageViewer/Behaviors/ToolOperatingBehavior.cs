using HPImageViewer.Tools;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;

namespace HPImageViewer.Behaviors
{
    internal class ToolOperatingBehavior : Behavior<ImageViewer>
    {
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            AssociatedObject.MouseWheel += AssociatedObject_MouseWheel;
            AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
        }


        private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            (ActivatedTool as IMouseTool)?.OnMouseWheel(imageView, e);

        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
                    (ActivatedTool as IMouseTool)?.OnMouseDoubleClick(imageView, e);
        }

        private void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            if (e.LeftButton == MouseButtonState.Released)
                ActivatedTool?.OnMouseUp(imageView, e);
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            ActivatedTool?.OnMouseMove(imageView, e);
        }

        private void AssociatedObject_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            if (e.LeftButton == MouseButtonState.Pressed)
                ActivatedTool?.OnMouseDown(imageView, e);
        }


        private ITool ActivatedTool => AssociatedObject.ActivatedToolInternal;





        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
            AssociatedObject.MouseWheel -= AssociatedObject_MouseWheel;

            base.OnDetaching();
        }


    }
}
