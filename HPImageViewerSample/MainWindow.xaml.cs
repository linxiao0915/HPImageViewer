using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

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
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }
        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private double _lastTime;
        private double _lowestFrameTime;
        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            double timeNow = _stopwatch.ElapsedMilliseconds;
            double elapsedMilliseconds = timeNow - _lastTime;
            _lowestFrameTime = Math.Min(_lowestFrameTime, elapsedMilliseconds);
            FpsCounter.Text = string.Format("FPS: {0:0.0} / Max: {1:0.0}", 1000.0 / elapsedMilliseconds, 1000.0 / _lowestFrameTime);
            _lastTime = timeNow;
        }


    }
}
