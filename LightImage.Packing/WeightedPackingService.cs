using System;
using System.Collections.Generic;
using System.Linq;

namespace LightImage.Packing
{
    public class WeightedPackingService : IWeightedPackingService
    {
        private IPackingService _service;

        public WeightedPackingService(IPackingService service)
        {
            _service = service;
        }

        public Packing<TItem> Pack<TItem>(IReadOnlyCollection<TItem> input, double width, double height, PackingOptions options) where TItem : IWeightedSize
        {
            options = new PackingOptions(options) { Smart = false };
            var items = input.Where(item => item.Width > 0 && item.Height > 0).Select(item => new Item<TItem>(item)).ToArray();
            if (items.Length == 0)
                return null;

            // Lowerbound: Put all items next to each other
            double maxFit = Math.Max(items.Sum(item => item.Width * item.Weight) / width, items.Sum(item => item.Height * item.Weight) / height);

            // Upperbound: Scale the largest item to fit width and height
            double minOverflow = items.Min(item => Math.Min(width / (item.Weight * item.Width), height / (item.Weight * item.Height)));

            Packing<Item<TItem>> best = default;
            bool isFit = false;
            while (best == null || minOverflow - maxFit > 0.01)
            {
                double middle = (minOverflow + maxFit) / 2;
                foreach (var item in items)
                    item.SetDiagonal(middle * item.Weight);
                var packing = _service.Pack(items, width, height, options);
                isFit = packing.Bins.Count == 1 && packing.Bins.Sum(b => b.Placements.Count) == items.Length;
                if (isFit)
                {
                    maxFit = middle;
                    best = packing;
                }
                else
                {
                    minOverflow = middle;
                }
            }
            return best.Transform(item => item.Source);
        }

        private class Item<TItem> : IWeightedSize where TItem : IWeightedSize
        {
            public Item(TItem source)
            {
                Width = source.Width;
                Height = source.Height;
                Weight = source.Weight;
                Source = source;
                Normalize();
            }

            public double Height { get; set; }
            public TItem Source { get; }
            public double Weight { get; set; } = 1.0;

            public double Width { get; set; }

            public void Normalize()
            {
                var diagonal = this.GetDiagonal();
                Scale(1 / diagonal);
            }

            public void Scale(double factor)
            {
                Width *= factor;
                Height *= factor;
            }

            public void SetDiagonal(double diagonal)
            {
                var current = this.GetDiagonal();
                Scale(diagonal / current);
            }
        }
    }
}