
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Vectors;

namespace Xylem.Graphics
{
    public sealed class ColorFormat
    {
        private static readonly Dictionary<string, ColorFormat> _index;

        public static readonly ColorFormat HSVA;
        public static readonly ColorFormat RGBA;

        static ColorFormat()
        {
            _index = new Dictionary<string, ColorFormat>();

            HSVA = new ColorFormat("hsva");
            RGBA = new ColorFormat("rgba");
        }

        public static ColorFormat Of(string identifier)
        {
            if (identifier == null || !_index.ContainsKey(identifier))
                return HSVA;
            
            return _index[identifier];
        }

        // 360 / 100 / 100 / 255
        public static Color FromHSV(int h, int s, int v, int a)
        {
            float S = (float) s / 100F;
            float V = (float) v / 100F;

            // Chroma
            float C = V * S;
            float H = (float) h / 60F;

            float X = C * (1 - System.Math.Abs((H % 2) - 1));

            float r = 0;
            float g = 0;
            float b = 0;

            if (0F <= H && H < 1F)
            {
                r = C;
                g = X;
                b = 0F;
            }

            else if (1F <= H && H < 2F)
            {
                r = X;
                g = C;
                b = 0F;
            }

            else if (2F <= H && H < 3F)
            {
                r = 0F;
                g = C;
                b = X;
            }

            else if (3F <= H && H < 4F)
            {
                r = 0F;
                g = X;
                b = C;
            }

            else if (4F <= H && H < 5F)
            {
                r = X;
                g = 0;
                b = C;
            }

            else if (5F <= H && H < 6F)
            {
                r = C;
                g = 0;
                b = X;
            }

            float m = V - C;

            int R = (int) System.Math.Round((r + m) * 255F);
            int G = (int) System.Math.Round((g + m) * 255F);
            int B = (int) System.Math.Round((b + m) * 255F);

            return new Color(R, G, B, a);
        }

        public static Vec3i ToHSV(Color color)
        {
            float R = (float) color.R / 255F;
            float G = (float) color.G / 255F;
            float B = (float) color.B / 255F;

            float v = System.Math.Max(System.Math.Max(R, G), B);
            float C = v - System.Math.Min(System.Math.Min(R, G), B);

            float h = 0F;
            float d = 0F;

            if (v == R)
                d = G - B;
            else if (v == G)
                d = (2F * C) + (B - R);
            else if (v == B)
                d = (4F * C) + (R - G);

            if (C != 0F)
                h = 60F * (d / C);

            float s = 0F;

            if (v != 0)
                s = C / v;

            int H = (int) System.Math.Round(h);
            int S = (int) System.Math.Round(s * 100F);
            int V = (int) System.Math.Round(v * 100F);

            return new Vec3i(H, S, V);
        }

        public readonly string Identifier;

        private ColorFormat(string identifier)
        {
            Identifier = identifier;
            _index[Identifier] = this;
        }
    }
}