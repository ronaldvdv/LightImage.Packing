using System.Collections.Generic;

namespace LightImage.Packing
{
    public interface IPackingService
    {
        Packing<TItem> Pack<TItem>(IReadOnlyCollection<TItem> input, double width, double height, PackingOptions options) where TItem : ISize;
    }
}