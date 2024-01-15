using HPImageViewer.Core;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace HPImageViewerSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(this.ImageControl);
            ImageControl.DocumentUpdated += ImageControl_DocumentUpdated;
            //CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        private void ImageControl_DocumentUpdated(object? sender, HPImageViewer.Core.Persistence.ImageViewerDesc e)
        {
            Console.WriteLine($"文档更新+{DateTime.Now}");
        }

        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private double _lastTime;
        private double _lowestFrameTime;
        //private void CompositionTarget_Rendering(object? sender, EventArgs e)
        //{
        //    var timeNow = _stopwatch.ElapsedMilliseconds;
        //    var elapsedMilliseconds = timeNow - _lastTime;
        //    _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
        //    FpsCounter.Text = string.Format("FPS: {0:0.0} / Max: {1:0.0}", 1000.0 / elapsedMilliseconds, 1000.0 / _lowestFrameTime);
        //    _lastTime = timeNow;
        //}

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {

                foreach (var item in ((Menu)menuItem.Parent).Items)
                {
                    ((MenuItem)item).IsChecked = false;
                }
                menuItem.IsChecked = true;

                if (menuItem.Tag is ToolType toolType)
                {
                    ImageControl.ActivatedTool = toolType;

                }
                else
                {
                    ImageControl.ActivatedTool = ToolType.None;

                }
            }
        }


    }
}
