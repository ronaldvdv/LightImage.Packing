using System;
using System.Collections.Generic;

namespace LightImage.Packing
{
    public class PackingOptions
    {
        internal Comparison<ISize> MaxAreaComparison = new Comparison<ISize>((a, b) => (int)(a.Width * a.Height - b.Width * b.Height));

        internal Comparison<ISize> MaxEdgeComparison = new Comparison<ISize>((a, b) => (int)(Math.Max(a.Width, a.Height) - Math.Max(b.Width, b.Height)));

        public PackingOptions()
        {
        }

        public PackingOptions(PackingOptions other)
        {
            AllowRotation = other.AllowRotation;
            Border = other.Border;
            Logic = other.Logic;
            Padding = other.Padding;
            PowerOfTwo = other.PowerOfTwo;
            Smart = other.Smart;
            Square = other.Square;
        }

        public bool AllowRotation { get; set; } = false;
        public double Border { get; set; } = 0;

        public Comparer<ISize> InputComparer
        {
            get
            {
                var comparison = Logic == PackingLogic.MaxArea ? MaxAreaComparison : MaxEdgeComparison;
                return Comparer<ISize>.Create(new Comparison<ISize>(comparison));
            }
        }

        public PackingLogic Logic { get; set; } = PackingLogic.MaxArea;
        public double Padding { get; set; } = 0;
        public bool PowerOfTwo { get; set; } = false;
        public bool Smart { get; set; } = false;
        public bool Square { get; set; } = false;
    }
}