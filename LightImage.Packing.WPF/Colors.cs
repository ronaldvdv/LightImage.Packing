using System;
using System.Linq;
using System.Windows.Media;

namespace LightImage.Packing.WPF
{
    public static class Colors
    {
        public static Color[] Material = new string[]
        {
                "F44336", // Red
                "E91E63", // Pink
                "9C27B0", // Purple
                "673AB7", // Deep purple
                "3F51B5", // Indigo
                "2196F3", // Blue
                "03A9F4", // Light blue
                "00BCD4", // Cyan
                "009688", // Teal
                "4CAF50", // Green
                "8BC34A", // Light green
                "CDDC39", // Lime
                "FFEB3B", // Yellow
                "FFC107", // Amber
                "FF9800", // Orange
                "FF5722", // Deep orange
                "795548", // Brown
                "9E9E9E", // Gray
                "607D8B"  // Blue gray
        }.Select(FromHex).ToArray();

        public static Color FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentNullException(nameof(hex));
            if (hex[0] == '#')
                hex = hex.Substring(1);
            int part = hex.Length > 3 ? 2 : 1;
            byte red = Convert.ToByte(hex.Substring(0 * part, part), 16);
            byte green = Convert.ToByte(hex.Substring(1 * part, part), 16);
            byte blue = Convert.ToByte(hex.Substring(2 * part, part), 16);
            return Color.FromRgb(red, green, blue);
        }
    }
}