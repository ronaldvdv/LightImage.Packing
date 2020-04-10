using System.Collections.Generic;

namespace LightImage.Packing
{
    public interface IWeightedPackingService
    {
        Packing<TItem> Pack<TItem>(IReadOnlyCollection<TItem> input, double width, double height, PackingOptions options) where TItem : IWeightedSize;
    }
}