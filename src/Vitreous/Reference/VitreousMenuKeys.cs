
using Microsoft.Xna.Framework;
using Xylem.Reference;
using Xylem.Data;

namespace Vitreous.Reference
{
    public static class MenuColors
    {
        public static readonly ColorPackage VitreousFrameworkColors = OptionPackage.Import<ColorPackage>("vitreous_menu_colors");

        public static Color Color(string key) => VitreousFrameworkColors[key];

        public static Color W1 = Color("w1");
        public static Color W2 = Color("w2");
        public static Color W3 = Color("w3");
        public static Color W4 = Color("w4");
        public static Color W5 = Color("w5");

        public static Color R1 = Color("r1");
        public static Color R2 = Color("r2");
        public static Color R3 = Color("r3");
        public static Color R4 = Color("r4");
        public static Color R5 = Color("r5");

        public static Color Y1 = Color("y1");
        public static Color Y2 = Color("y2");
        public static Color Y3 = Color("y3");
        public static Color Y4 = Color("y4");
        public static Color Y5 = Color("y5");

        public static Color B5 = Color("b5");
    }
}