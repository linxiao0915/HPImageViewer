using System;

namespace HPImageViewer.Core.Primitives
{
    public class Rect
    {
        internal double _x;
        internal double _y;
        internal double _width;
        internal double _height;
        public static Rect Empty => Rect.s_empty;

        public Rect()
        {

        }
        private static readonly Rect s_empty = CreateEmptyRect(new Rect());
        public Rect(double x, double y, double width, double height)
        {
            if (width < 0.0 || height < 0.0)
                throw new ArgumentException("Size_WidthAndHeightCannotBeNegative");
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Rect" /> structure that is exactly large enough to contain the two specified points.</summary>
        /// <param name="point1">The first point that the new rectangle must contain.</param>
        /// <param name="point2">The second point that the new rectangle must contain.</param>
        public Rect(Point point1, Point point2)
        {
            this._x = Math.Min(point1.X, point2.X);
            this._y = Math.Min(point1.Y, point2.Y);
            this._width = Math.Max(Math.Max(point1.X, point2.X) - this._x, 0.0);
            this._height = Math.Max(Math.Max(point1.Y, point2.Y) - this._y, 0.0);
        }

        /// <summary>Gets or sets the x-axis value of the left side of the rectangle.</summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// <see cref="P:System.Windows.Rect.X" /> is set on an <see cref="P:System.Windows.Rect.Empty" /> rectangle.</exception>
        /// <returns>The x-axis value of the left side of the rectangle.</returns>
        public double X
        {
            get => this._x;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Rect_CannotModifyEmptyRect");
                this._x = value;
            }
        }

        /// <summary>Gets or sets the y-axis value of the top side of the rectangle.</summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// <see cref="P:System.Windows.Rect.Y" /> is set on an <see cref="P:System.Windows.Rect.Empty" /> rectangle.</exception>
        /// <returns>The y-axis value of the top side of the rectangle.</returns>
        public double Y
        {
            get => this._y;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Rect_CannotModifyEmptyRect");
                this._y = value;
            }
        }

        /// <summary>Gets or sets the width of the rectangle.</summary>
        /// <exception cref="T:System.ArgumentException">
        /// <see cref="P:System.Windows.Rect.Width" /> is set to a negative value.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// <see cref="P:System.Windows.Rect.Width" /> is set on an <see cref="P:System.Windows.Rect.Empty" /> rectangle.</exception>
        /// <returns>A positive number that represents the width of the rectangle. The default is 0.</returns>
        public double Width
        {
            get => this._width;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Rect_CannotModifyEmptyRect");
                this._width = value >= 0.0 ? value : throw new ArgumentException("Size_WidthCannotBeNegative");
            }
        }

        /// <summary>Gets or sets the height of the rectangle.</summary>
        /// <exception cref="T:System.ArgumentException">
        /// <see cref="P:System.Windows.Rect.Height" /> is set to a negative value.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// <see cref="P:System.Windows.Rect.Height" /> is set on an <see cref="P:System.Windows.Rect.Empty" /> rectangle.</exception>
        /// <returns>A positive number that represents the height of the rectangle. The default is 0.</returns>
        public double Height
        {
            get => this._height;
            set
            {
                if (this.IsEmpty)
                    throw new InvalidOperationException("Rect_CannotModifyEmptyRect");
                this._height = value >= 0.0 ? value : throw new ArgumentException("Size_HeightCannotBeNegative");
            }
        }
        public bool IsEmpty => this._width < 0.0;

        /// <summary>Gets the x-axis value of the left side of the rectangle.</summary>
        /// <returns>The x-axis value of the left side of the rectangle.</returns>
        public double Left => this._x;

        /// <summary>Gets the y-axis position of the top of the rectangle.</summary>
        /// <returns>The y-axis position of the top of the rectangle.</returns>
        public double Top => this._y;

        /// <summary>Gets the x-axis value of the right side of the rectangle.</summary>
        /// <returns>The x-axis value of the right side of the rectangle.</returns>
        public double Right => this.IsEmpty ? double.NegativeInfinity : this._x + this._width;

        /// <summary>Gets the y-axis value of the bottom of the rectangle.</summary>
        /// <returns>The y-axis value of the bottom of the rectangle. If the rectangle is empty, the value is <see cref="F:System.Double.NegativeInfinity" /> .</returns>
        public double Bottom => this.IsEmpty ? double.NegativeInfinity : this._y + this._height;

        /// <summary>Gets the position of the top-left corner of the rectangle.</summary>
        /// <returns>The position of the top-left corner of the rectangle.</returns>
        public Point TopLeft => new Point(this.Left, this.Top);

        /// <summary>Gets the position of the top-right corner of the rectangle.</summary>
        /// <returns>The position of the top-right corner of the rectangle.</returns>
        public Point TopRight => new Point(this.Right, this.Top);

        /// <summary>Gets the position of the bottom-left corner of the rectangle.</summary>
        /// <returns>The position of the bottom-left corner of the rectangle.</returns>
        public Point BottomLeft => new Point(this.Left, this.Bottom);

        /// <summary>Gets the position of the bottom-right corner of the rectangle.</summary>
        /// <returns>The position of the bottom-right corner of the rectangle.</returns>
        public Point BottomRight => new Point(this.Right, this.Bottom);

        public bool IntersectsWith(Rect rect) => !this.IsEmpty && !rect.IsEmpty && rect.Left <= this.Right && rect.Right >= this.Left && rect.Top <= this.Bottom && rect.Bottom >= this.Top;

        public void Intersect(Rect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                CreateEmptyRect(this);
            }
            else
            {
                double num1 = Math.Max(this.Left, rect.Left);
                double num2 = Math.Max(this.Top, rect.Top);
                this._width = Math.Max(Math.Min(this.Right, rect.Right) - num1, 0.0);
                this._height = Math.Max(Math.Min(this.Bottom, rect.Bottom) - num2, 0.0);
                this._x = num1;
                this._y = num2;
            }
        }

        private static Rect CreateEmptyRect(Rect rect)
        {
            rect._x = double.PositiveInfinity;
            rect._y = double.PositiveInfinity;
            rect._width = double.NegativeInfinity;
            rect._height = double.NegativeInfinity;
            return rect;
        }
        private bool ContainsInternal(double x, double y) => x >= this._x && x - this._width <= this._x && y >= this._y && y - this._height <= this._y;
        /// <summary>Indicates whether the rectangle contains the specified point.</summary>
        /// <param name="point">The point to check.</param>
        /// <returns>
        /// <see langword="true" /> if the rectangle contains the specified point; otherwise, <see langword="false" />.</returns>
        public bool Contains(Point point) => this.Contains(point.X, point.Y);

        /// <summary>Indicates whether the rectangle contains the specified x-coordinate and y-coordinate.</summary>
        /// <param name="x">The x-coordinate of the point to check.</param>
        /// <param name="y">The y-coordinate of the point to check.</param>
        /// <returns>
        /// <see langword="true" /> if (<paramref name="x" />, <paramref name="y" />) is contained by the rectangle; otherwise, <see langword="false" />.</returns>
        public bool Contains(double x, double y) => !this.IsEmpty && this.ContainsInternal(x, y);

        /// <summary>Indicates whether the rectangle contains the specified rectangle.</summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="rect" /> is entirely contained by the rectangle; otherwise, <see langword="false" />.</returns>
        public bool Contains(Rect rect) => !this.IsEmpty && !rect.IsEmpty && this._x <= rect._x && this._y <= rect._y && this._x + this._width >= rect._x + rect._width && this._y + this._height >= rect._y + rect._height;
    }
}
