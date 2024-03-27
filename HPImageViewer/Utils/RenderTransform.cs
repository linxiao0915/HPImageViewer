using System.Windows;
using System.Windows.Media;

namespace HPImageViewer.Utils;

internal class RenderTransform : ICoordTransform
{

    public Matrix Matrix { get; set; }
    public RenderTransform(Matrix matrix)
    {
        Matrix = matrix;
    }
    //public void ToDomain(double dx, double dy, out double wx, out double wy)
    //{
    //    var matrix = _matrix;
    //    matrix.Invert();
    //    var point = matrix.Transform(new Point(dx, dy));
    //    wx = point.X; wy = point.Y;

    //}

    //public void ToDevice(double wx, double wy, out double dx, out double dy)
    //{
    //    var matrix = _matrix;
    //    var point = matrix.Transform(new Point(wx, wy));
    //    dx = point.X; dy = point.Y;
    //}

    public HPImageViewer.Core.Primitives.Point ToDomain(HPImageViewer.Core.Primitives.Point devicePoint)
    {
        var matrix = Matrix;
        matrix.Invert();
        return matrix.Transform(devicePoint.ToWindowPoint()).ToPoint();
    }

    public HPImageViewer.Core.Primitives.Point ToDevice(HPImageViewer.Core.Primitives.Point worldPoint)
    {
        var matrix = Matrix;
        return matrix.Transform(worldPoint.ToWindowPoint()).ToPoint();
    }

    public Vector ToDomain(Vector deviceVector)
    {
        var matrix = Matrix;
        matrix.Invert();
        return matrix.Transform(deviceVector);
    }

    public Vector ToDevice(Vector worldVector)
    {
        var matrix = Matrix;
        return matrix.Transform(worldVector);
    }
}