
using Microsoft.Xna.Framework;

namespace Xylem.Graphics
{
    public static class ColorExtensions
    {
        public static Color Add(this Color color, Color other)
        {
            return new Color(color.A + other.A, color.G + other.G, color.B + other.B, color.A + other.A);
        }

        public static Color Subtract(this Color color, Color other)
        {
            return new Color(color.A - other.A, color.G - other.G, color.B - other.B, color.A - other.A);
        }
    }
}