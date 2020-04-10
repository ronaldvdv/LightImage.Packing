using PropertyChanged;
using System;
using System.Collections.ObjectModel;

namespace LightImage.Packing
{
    [AddINotifyPropertyChangedInterface]
    public abstract class Bin<TItem> where TItem : ISize
    {
        private ObservableCollection<Placement<TItem>> _placements = new ObservableCollection<Placement<TItem>>();

        internal Bin()
        {
            Placements = new ReadOnlyObservableCollection<Placement<TItem>>(_placements);
        }

        public double Height { get; protected set; }
        public ReadOnlyObservableCollection<Placement<TItem>> Placements { get; }
        public double Width { get; protected set; }

        public abstract Placement<TItem> Add(TItem item);

        internal abstract Bin<TNewItem> Transform<TNewItem>(Func<TItem, TNewItem> transform) where TNewItem : ISize;

        protected Placement<TItem> AddPlacement(Rectangle rectangle, TItem data)
        {
            var placement = new Placement<TItem>(rectangle, data, this);
            _placements.Add(placement);
            return placement;
        }
    }
}