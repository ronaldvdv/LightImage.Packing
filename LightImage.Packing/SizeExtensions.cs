using System;

namespace LightImage.Packing
{
    public static class SizeExtensions
    {
        public static double GetArea(this ISize size) => size.Width * size.Height;

        public static double GetDiagonal(this ISize size) => Math.Sqrt(size.Width * size.Width + size.Height * size.Height);

        public static Rectangle Place(this ISize size, double left, double top)
        {
            return new Rectangle(left, top, size.Width, size.Height);
        }
    }
}