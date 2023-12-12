using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace HPImageViewer.Converters
{
    public class BrushRoundConverter : IValueConverter
    {
        public Brush HighValue { get; set; } = (Brush)Brushes.White;

        public Brush LowValue { get; set; } = (Brush)Brushes.Black;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SolidColorBrush solidColorBrush))
                return Binding.DoNothing;
            Color color = solidColorBrush.Color;
            return 0.3 * (double)color.R + 0.59 * (double)color.G + 0.11 * (double)color.B >= 123.0 ? (object)this.HighValue : (object)this.LowValue;
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
