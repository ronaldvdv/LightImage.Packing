using System;
using System.Linq;

namespace LightImage.Packing
{
    internal class OversizedElementBin<TItem> : Bin<TItem> where TItem : ISize
    {
        internal OversizedElementBin(TItem data) : base()
        {
            AddPlacement(data.Place(0, 0), data);
        }

        public override Placement<TItem> Add(TItem item)
        {
            return null;
        }

        internal override Bin<TNewItem> Transform<TNewItem>(Func<TItem, TNewItem> transform)
        {
            return new OversizedElementBin<TNewItem>(transform(Placements.Single().Data));
        }
    }
}