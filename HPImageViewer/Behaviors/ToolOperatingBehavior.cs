using HPImageViewer.Tools;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HPImageViewer.Behaviors
{
    internal class ToolOperatingBehavior : Behavior<ImageControl>
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
            AssociatedObject.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(MenuItem_Click));
        }

        private void AssociatedObject_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            (_activedTool as IMouseWheelTool)?.OnMouseWheel(imageView, e);

        }

        private void AssociatedObject_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            if (e.LeftButton == MouseButtonState.Released)
                _activedTool?.OnMouseUp(imageView, e);
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            _activedTool?.OnMouseMove(imageView, e);
        }

        private void AssociatedObject_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ImageView imageView == false) return;
            if (e.LeftButton == MouseButtonState.Pressed)
                _activedTool?.OnMouseDown(imageView, e);
        }

        private ITool _activedTool;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {

                foreach (var item in ((Menu)menuItem.Parent).Items)
                {
                    ((MenuItem)item).IsChecked = false;
                }
                menuItem.IsChecked = true;
                if (menuItem.Tag is Type t && t.IsAssignableTo(typeof(ITool)))
                {
                    if (t != _activedTool?.GetType())
                    {
                        _activedTool = (ITool)Activator.CreateInstance(t);
                    }
                }
                else
                {
                    _activedTool = null;
                }
            }
        }

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
