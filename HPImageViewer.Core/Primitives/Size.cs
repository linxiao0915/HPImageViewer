
using System;

namespace HPImageViewer.Core.Primitives
{
    public class Size
    {
        double _width;
        double _height;

        public Size()
        {

        }
        public Size(double width, double height)
        {
            this.Width = width >= 0.0 && height >= 0.0 ? width : throw new ArgumentException("Size_WidthAndHeightCannotBeNegative");
            this.Height = height;
        }



        public double Width
        {
            get => this._width;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Size_CannotModifyEmptySize");
                this._width = value >= 0.0 ? value : throw new ArgumentException("Size_WidthCannotBeNegative");
            }
        }

        /// <summary>Gets or sets the <see cref="P:System.Windows.Size.Height" /> of this instance of <see cref="T:System.Windows.Size" />.</summary>
        /// <returns>The <see cref="P:System.Windows.Size.Height" /> of this instance of <see cref="T:System.Windows.Size" />. The default is 0. The value cannot be negative.</returns>
        public double Height
        {
            get => this._height;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Size_CannotModifyEmptySize");
                this._height = value >= 0.0 ? value : throw new ArgumentException("Size_HeightCannotBeNegative");
            }
        }

        private static readonly Size s_empty = Size.CreateEmptySize();


        private static Size CreateEmptySize() => new Size()
        {
            _width = double.NegativeInfinity,
            _height = double.NegativeInfinity
        };

        public bool IsEmpty => this._width < 0.0;

        public override string ToString()
        {
            return $"({Width},{Height})";
        }
    }
}
