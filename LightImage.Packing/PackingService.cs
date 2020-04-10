using System.Collections.Generic;

namespace LightImage.Packing
{
    public class PackingService : IPackingService
    {
        public Packing<TItem> Pack<TItem>(IReadOnlyCollection<TItem> input, double width, double height, PackingOptions options) where TItem : ISize
        {
            var packing = new Packing<TItem>(width, height, options);
            packing.Add(input);
            return packing;
        }
    }
}