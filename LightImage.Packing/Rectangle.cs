namespace LightImage.Packing
{
    public sealed class Rectangle
    {
        public Rectangle(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Right = left + width;
            Bottom = top + height;
        }

        public double Bottom
        {
            get => Top + Height;
            set => Height = value - Top;
        }

        public double Height { get; set; }
        public double Left { get; set; }

        public double Right
        {
            get => Left + Width;
            set => Width = value - Left;
        }

        public double Top { get; set; }
        public double Width { get; set; }

        public bool Contains(Rectangle other)
        {
            return other.Left >= Left && other.Right <= Right && other.Top >= Top && other.Bottom <= Bottom;
        }

        public bool Overlaps(Rectangle other)
        {
            return other.Left < Right && other.Right > Left && other.Top < Bottom && other.Bottom > Top;
        }
    }
}