using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LightImage.Packing
{
    [AddINotifyPropertyChangedInterface]
    public class Packing<TItem> where TItem : ISize
    {
        private static EqualityComparer<TItem> _comparer = EqualityComparer<TItem>.Default;

        private readonly ObservableCollection<Bin<TItem>> _bins = new ObservableCollection<Bin<TItem>>();
        private int _currentBin = 0;

        internal Packing(double width, double height, PackingOptions options)
        {
            Width = width;
            Height = height;
            Options = options;
            Bins = new ReadOnlyObservableCollection<Bin<TItem>>(_bins);
        }

        public ReadOnlyObservableCollection<Bin<TItem>> Bins { get; }
        public double Height { get; }
        public PackingOptions Options { get; }
        public double Width { get; }

        public Placement<TItem> this[TItem item]
        {
            get
            {
                return _bins.Select(bin => bin.Placements.FirstOrDefault(placement => _comparer.Equals(placement.Data, item))).FirstOrDefault(placement => placement != null);
            }
        }

        public void Add(TItem item)
        {
            if (item.Width > Width || item.Height > Height)
            {
                var bin = new OversizedElementBin<TItem>(item);
                _bins.Add(bin);
            }
            else
            {
                var placement = _bins.Skip(_currentBin).FirstOrDefault(b => b.Add(item) != null);
                if (placement == null)
                {
                    var bin = new MaxRectBin<TItem>(Width, Height, Options);
                    bin.Add(item);
                    _bins.Add(bin);
                }
            }
        }

        public void Add(IReadOnlyCollection<TItem> input)
        {
            input = input.OrderByDescending(x => (ISize)x, Options.InputComparer).ToArray();
            foreach (var item in input)
                Add(item);
        }

        public void Next()
        {
            _currentBin = _bins.Count;
        }

        public void Reset()
        {
            _bins.Clear();
            _currentBin = 0;
        }

        public Packing<TNewItem> Transform<TNewItem>(Func<TItem, TNewItem> transform) where TNewItem : ISize
        {
            var result = new Packing<TNewItem>(Width, Height, Options);
            foreach (var bin in _bins)
                result.Add(bin.Transform(transform));
            return result;
        }

        internal void Add(Bin<TItem> bin)
        {
            _bins.Add(bin);
        }
    }
}