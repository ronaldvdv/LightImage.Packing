using System;
using System.Collections.Generic;
using System.Linq;

namespace LightImage.Packing
{
    internal class MaxRectBin<TItem> : Bin<TItem> where TItem : ISize
    {
        private readonly List<Rectangle> _freeRectangles = new List<Rectangle>();

        private readonly PackingOptions _options;

        private Rectangle _stage;

        internal MaxRectBin(double maxWidth, double maxHeight, PackingOptions options) : base()
        {
            _options = options;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            Reset();
        }

        public double Border => _options.Border;
        public double MaxHeight { get; }
        public double MaxWidth { get; }
        public double Padding => _options.Padding;
        public bool VerticalExpand { get; private set; } = false;

        public override Placement<TItem> Add(TItem input)
        {
            var result = Place(input);
            if (result == null)
            {
                return null;
            }

            return AddPlacement(result, input);
        }

        public IReadOnlyCollection<TItem> Add(IEnumerable<TItem> input)
        {
            var items = input.OrderByDescending(item => item, _options.InputComparer);
            var unpacked = new List<TItem>();
            foreach (var item in input)
            {
                var placement = Add(item);
                if (placement == null)
                    unpacked.Add(item);
            }
            return unpacked;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Rectangles that could not be placed</returns>
        public IReadOnlyCollection<TItem> Repack()
        {
            var input = Placements.Select(p => p.Data).ToArray();
            Reset();
            return Add(input);
        }

        internal override Bin<TNewItem> Transform<TNewItem>(Func<TItem, TNewItem> transform)
        {
            var bin = new MaxRectBin<TNewItem>(MaxWidth, MaxHeight, _options);
            foreach (var placement in Placements)
                bin.AddPlacement(placement.Rectangle, transform(placement.Data));
            return bin;
        }

        private void ExpandFreeRects(double width, double height)
        {
            foreach (var freeRect in _freeRectangles)
            {
                if (freeRect.Right >= Math.Min(Width + Padding - Border, width))
                {
                    freeRect.Width = Width - freeRect.Left - Border;
                }
                if (freeRect.Bottom >= Math.Min(Height + Padding - Border, height))
                {
                    freeRect.Height = height - freeRect.Top - Border;
                }
            }

            _freeRectangles.Add(new Rectangle(Width + Padding - Border, Border, width - Width - Padding, height - Border * 2));
            _freeRectangles.Add(new Rectangle(Border, Height + Padding - Border, width - Border * 2, height - Height - Padding));
            _freeRectangles.RemoveAll(r => r.Width <= 0 || r.Height <= 0 || r.Left < Border || r.Top < Border);
            PruneFreeRectangleList();
        }

        private Rectangle FindNode(double width, double height)
        {
            double score = double.MaxValue;
            double areaFit;
            Rectangle bestNode = null;
            foreach (var rect in _freeRectangles)
            {
                if (rect.Width >= width && rect.Height >= height)
                {
                    areaFit = (_options.Logic == PackingLogic.MaxArea) ? rect.Width * rect.Height - width * height : Math.Min(rect.Width - width, rect.Height - height);
                    if (areaFit < score)
                    {
                        bestNode = new Rectangle(rect.Left, rect.Top, width, height);
                        score = areaFit;
                    }
                }

                if (!_options.AllowRotation)
                {
                    continue;
                }

                // Continue to test 90-degree rotated rectangle
                if (rect.Width >= height && rect.Height >= width)
                {
                    areaFit = (_options.Logic == PackingLogic.MaxArea) ? rect.Width * rect.Height - height * width : Math.Min(rect.Height - width, rect.Width - height);
                    if (areaFit < score)
                    {
                        bestNode = new Rectangle(rect.Left, rect.Top, height, width);
                        score = areaFit;
                    }
                }
            }
            return bestNode;
        }

        private Rectangle Place(ISize size)
        {
            var node = FindNode(size.Width + Padding, size.Height + Padding);
            if (node != null)
            {
                UpdateBinSize(node);
                var numRectToProcess = _freeRectangles.Count;
                int i = 0;
                while (i < numRectToProcess)
                {
                    if (SplitNode(_freeRectangles[i], node))
                    {
                        _freeRectangles.RemoveAt(i);
                        numRectToProcess--;
                        i--;
                    }
                    i++;
                }
                PruneFreeRectangleList();
                VerticalExpand = Width > Height ? true : false;
                var result = new Rectangle(node.Left, node.Top, size.Width, size.Height);
                // TODO rect.rot = node.rot ? !rect.rot : rect.rot;
                // this._dirty++;
                return result;
            }
            else if (!VerticalExpand)
            {
                if (UpdateBinSize(new Rectangle(
                    Width + Padding - Border, Border,
                    size.Width + Padding, size.Height + Padding
                )) || UpdateBinSize(new Rectangle(
                    Border, Height + Padding - Border,
                    size.Width + Padding, size.Height + Padding
                )))
                {
                    return Place(size);
                }
            }
            else
            {
                if (UpdateBinSize(new Rectangle(
                    Border, Height + Padding - Border,
                    size.Width + Padding, size.Height + Padding
                )) || UpdateBinSize(new Rectangle(
                    Width + Padding - Border, Border,
                    size.Width + Padding, size.Height + Padding
                )))
                {
                    return Place(size);
                }
            }
            return null;
        }

        private void PruneFreeRectangleList()
        {
            // Go through each pair of freeRects and remove any rects that is redundant
            int i = 0;
            int j = 0;
            int len = _freeRectangles.Count;
            while (i < len)
            {
                j = i + 1;
                var tmpRect1 = _freeRectangles[i];
                while (j < len)
                {
                    var tmpRect2 = _freeRectangles[j];
                    if (tmpRect2.Contains(tmpRect1))
                    {
                        _freeRectangles.RemoveAt(i);
                        i--;
                        len--;
                        break;
                    }
                    if (tmpRect1.Contains(tmpRect2))
                    {
                        _freeRectangles.RemoveAt(j);
                        j--;
                        len--;
                    }
                    j++;
                }
                i++;
            }
        }

        private void Reset()
        {
            Width = _options.Smart ? 0 : MaxWidth;
            Height = _options.Smart ? 0 : MaxHeight;
            _stage = new Rectangle(0, 0, Width, Height);
            _freeRectangles.Clear();
            _freeRectangles.Add(new Rectangle(Border, Border, MaxWidth + Padding - 2 * Border, MaxHeight + Padding - 2 * Border));
        }

        private bool SplitNode(Rectangle freeRect, Rectangle usedNode)
        {
            // Test if usedNode intersect with freeRect
            if (!freeRect.Overlaps(usedNode))
            {
                return false;
            }

            // Do vertical split
            if (usedNode.Left < freeRect.Right && usedNode.Right > freeRect.Left)
            {
                // New node at the top side of the used node
                if (usedNode.Top > freeRect.Top && usedNode.Top < freeRect.Bottom)
                {
                    var newNode = new Rectangle(freeRect.Left, freeRect.Top, freeRect.Width, usedNode.Top - freeRect.Top);
                    _freeRectangles.Add(newNode);
                }

                // New node at the bottom side of the used node
                if (usedNode.Bottom < freeRect.Bottom)
                {
                    var newNode = new Rectangle(freeRect.Left, usedNode.Bottom, freeRect.Width, freeRect.Bottom - usedNode.Bottom);
                    _freeRectangles.Add(newNode);
                }
            }

            // Do horizontal split
            if (usedNode.Top < freeRect.Bottom && usedNode.Bottom > freeRect.Top)
            {
                // New node at the left side of the used node.
                if (usedNode.Left > freeRect.Left && usedNode.Left < freeRect.Right)
                {
                    var newNode = new Rectangle(freeRect.Left, freeRect.Top, usedNode.Left - freeRect.Left, freeRect.Height);
                    _freeRectangles.Add(newNode);
                }

                // New node at the right side of the used node.
                if (usedNode.Right < freeRect.Right)
                {
                    var newNode = new Rectangle(usedNode.Right, freeRect.Top, freeRect.Right - usedNode.Right, freeRect.Height);
                    _freeRectangles.Add(newNode);
                }
            }

            return true;
        }

        private bool UpdateBinSize(Rectangle node)
        {
            if (!_options.Smart)
            {
                return false;
            }

            if (_stage.Contains(node))
            {
                return false;
            }

            var tmpWidth = Math.Max(Width, node.Right - Padding + Border);
            var tmpHeight = Math.Max(Height, node.Bottom - Padding + Border);
            if (_options.AllowRotation)
            {
                // do extra test on rotated node whether it's a better choice
                var rotWidth = Math.Max(Width, node.Left + node.Height - Padding + Border);
                var rotHeight = Math.Max(Height, node.Top + node.Width - Padding + Border);
                if (rotWidth * rotHeight < tmpWidth * tmpHeight)
                {
                    tmpWidth = rotWidth;
                    tmpHeight = rotHeight;
                }
            }
            if (_options.PowerOfTwo)
            {
                tmpWidth = Math.Pow(2, Math.Ceiling(Math.Log(tmpWidth, 2)));
                tmpHeight = Math.Pow(2, Math.Ceiling(Math.Log(tmpHeight, 2)));
            }
            if (_options.Square)
            {
                tmpWidth = tmpHeight = Math.Max(tmpWidth, tmpHeight);
            }
            if (tmpWidth > MaxWidth + Padding || tmpHeight > MaxHeight + Padding)
            {
                return false;
            }
            ExpandFreeRects(tmpWidth + Padding, tmpHeight + Padding);
            Width = _stage.Width = tmpWidth;
            Height = _stage.Height = tmpHeight;
            return true;
        }
    }
}