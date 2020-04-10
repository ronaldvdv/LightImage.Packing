namespace LightImage.Packing
{
    public sealed class Placement<TData> where TData : ISize
    {
        internal Placement(Rectangle rectangle, TData data, Bin<TData> bin)
        {
            Data = data;
            Bin = bin;
            Rectangle = rectangle;
        }

        public Bin<TData> Bin { get; }
        public TData Data { get; }
        public Rectangle Rectangle { get; }
    }
}