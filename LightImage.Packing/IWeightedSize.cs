namespace LightImage.Packing
{
    public interface IWeightedSize : ISize
    {
        double Weight { get; }
    }
}