using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using System.Windows.Media;

namespace LightImage.Packing.WPF
{
    public class ViewModel : ReactiveObject
    {
        public ViewModel()
        {
            RunWeighted = ReactiveCommand.Create(RunWeightedImpl);
            RunUnweighted = ReactiveCommand.Create(RunUnweightedImpl);
        }

        [Reactive]
        public int Count { get; set; } = 100;

        [Reactive]
        public double Height { get; set; } = 600;

        public PackingOptions Options { get; } = new PackingOptions();

        [Reactive]
        public Packing<Box> Packing { get; private set; }

        public ReactiveCommand<Unit, Unit> RunUnweighted { get; }
        public ICommand RunWeighted { get; }

        [Reactive]
        public Bin<Box> SelectedBin { get; set; }

        [Reactive]
        public double Width { get; set; } = 800;

        private IReadOnlyCollection<Box> CreateInput()
        {
            var input = new List<Box>();
            var random = new Random();
            for (int i = 0; i < Count; i++)
            {
                var width = random.Next(40, 100);
                var height = random.Next(40, 100);
                var color = Colors.Material[random.Next(0, Colors.Material.Length)];
                var box = new Box(width, height, color);
                box.Weight = random.NextDouble() > 0.9 ? 4 : (random.NextDouble() > 0.75 ? 2 : 1);
                input.Add(box);
            }

            return input;
        }

        private void RunUnweightedImpl()
        {
            var packer = new PackingService();
            var input = CreateInput();
            Packing = packer.Pack(input, Width, Height, Options);
            SelectedBin = Packing.Bins.FirstOrDefault();
        }

        private void RunWeightedImpl()
        {
            var packer = new PackingService();
            var service = new WeightedPackingService(packer);
            var input = CreateInput();
            Packing = service.Pack(input, Width, Height, Options);
            SelectedBin = Packing.Bins.FirstOrDefault();
        }

        public class Box : IWeightedSize
        {
            public Box(double width, double height, Color color)
            {
                Width = width;
                Height = height;
                Color = color;
            }

            public Color Color { get; }
            public double Height { get; set; }
            public double Weight { get; set; } = 1.0;
            public double Width { get; set; }
        }
    }
}