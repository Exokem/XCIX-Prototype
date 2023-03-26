
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xylem.Graphics;
using Xylem.Registration;
using Xylem.Reference;
using Xylem.Input;

namespace Xylem.Component
{
    public class Typeface : RegistryEntry
    {
        public static readonly Color ColorGrid = Color.Black;
        public static readonly Rectangle MissingArea = new Rectangle();

        /* Glyph data mappings for each character included in the typeface. */
        protected readonly Dictionary<char, Rectangle> _glyphs = 
            new Dictionary<char, Rectangle>();

        protected readonly Texture2D _texture;
        
        public readonly bool Monospace;
        public readonly int MaxWidth;
        public readonly int Height;
        public readonly int Space;

        /* The base scale for the texture - ensures uniform base height across types. */
        public readonly float BaseScale;

        public Typeface (JObject data) : base(data)
        {
            Texture2D texture = R.Textures[J.ReadString(data, K.Resource)].Texture;

            Height = J.ReadInt(data, K.Height);
            Space = J.ReadInt(data, K.Space, 1);

            // TODO: adjust scaling 
            // BaseScale = (float) Math.Ceiling(16F / (float) Height);
            BaseScale = J.ReadFloat(data, K.BaseScale, 1F);
            // Output.Write($"{Identifier}: using scale {BaseScale}");

            Queue<char> characters = new Queue<char>();
            List<int> rowCounts = new List<int>();
            J.ReadArrayTokens(data, K.Layout, token => 
            {
                string line = token.ToString();
                char[] chars = line.ToCharArray();
                rowCounts.Add(chars.Length);
                foreach (char c in chars)
                    characters.Enqueue(c);
            });

            _texture = texture;

            // Read color data
            Color[] pixels = new Color[_texture.Width * _texture.Height];
            _texture.GetData<Color>(pixels);

            int maxWidth = 0;
            bool monospace = true;
            int row = 0;

            // Start at y:1 because the first row will be the grid color
            for (int y = 1; y < _texture.Height && characters.Count != 0;)
            {
                Rectangle area = new Rectangle();
                bool scanning = false;

                for (int x = 0; x < _texture.Width && characters.Count != 0 && rowCounts[row] != 0; x ++)
                {
                    Color color = pixels[_texture.Width * y + x];

                    if (scanning)
                    {
                        if (color == ColorGrid)
                        {
                            // Finish scan
                            if (maxWidth != area.Width)
                                monospace = false;

                            char c = characters.Dequeue();
                            maxWidth = System.Math.Max(maxWidth, area.Width);
                            _glyphs[c] = area;
                            rowCounts[row]--;
                            scanning = false;
                            // Output.Write($"[XCIX/Typeface] {Identifier}: Assigned region {area} for character {c}");
                        }

                        else
                        {
                            // Continue scan
                            area.Width ++;
                        }
                    }

                    else if (color != ColorGrid)
                    {
                        // Start scan
                        area = new Rectangle(x, y, 1, Height);
                        scanning = true;
                    }
                }

                y += Height + 1;
                row ++;
            }

            MaxWidth = maxWidth;
            Monospace = monospace;
        }

        /* A char is supported if it is included in the 'layout' of the format JSON. */
        public bool SupportsChar(char c) => _glyphs.ContainsKey(c);

        /* Provides the area of the typeface texture allocated to the specified char. */
        protected Rectangle CharSourceArea(char glyph) =>
            _glyphs.ContainsKey(glyph) ? _glyphs[glyph] : MissingArea;

        /**
         * Provides a scaled destination rectangle for a given character to be rendered.
         */
        public Rectangle CharArea(char glyph, int x, int y, float scale = 1.0F)
        {
            if (!_glyphs.ContainsKey(glyph))
                return MissingArea;
            else return new Rectangle(x, y, (int) CharWidth(glyph, scale), (int) CharHeight(scale));
        }

        public int RenderChar(char c, int x, int y, SpriteBatch renderer, float scale = 1.0F) => RenderChar(c, x, y, renderer, Color.White, scale);

        public int ScaledSpaceWidth(float scale = 1.0F) => (int) ((float) Space * BaseScale * scale);

        public int ScaledHeight(float scale = 1.0F) => (int) ((float) Height * BaseScale * scale);

        /**
         * Renders a character at a specified position, automatically scaling the 
         * destination area as necessary. 
         *
         * The returned value is the width of the drawn character plus the standard space
         * between characters, both scaled. This is useful for functions that use this one
         * to render a sequence of characters in the same line. 
         */
        public int RenderChar(char c, int x, int y, SpriteBatch renderer, Color color, float scale = 1.0F, bool ignoreDebug = false)
        {
            if (!SupportsChar(c))
                return 0;

            Rectangle src = CharSourceArea(c);
            Rectangle dst = CharArea(c, x, y, scale);

            renderer.Draw(_texture, dst, src, color);

            if (GraphicsConfiguration.Debug && !ignoreDebug && dst.Contains(InputProcessor.X, InputProcessor.Y))
                Pixel.FrameRect(dst, Color.BlueViolet);

            return dst.Width + (int) (Space * BaseScale * scale);
        }

        /* Provides the scaled width of a specified character. */
        public float CharWidth(char glyph, float scale = 1.0F) => 
            _glyphs.ContainsKey(glyph) ? _glyphs[glyph].Width * BaseScale * scale : 0;

        /* Provides the scaled width of a specified character plus a leading 'space'. */
        public float SpacedCharWidth(char glyph, float scale = 1.0F)
        {
            return CharWidth(glyph, scale) + (Space * BaseScale * scale);
        }

        /* The scaled height of all characters will be the same for one typeface. */
        public float CharHeight(float scale = 1.0F) => 
            Height * BaseScale * scale;

        /* Calculates the scaled length of a string, including character spaces. */
        public float StringWidth(string text, float scale = 1.0F)
        {
            float length = 0;
            foreach (char c in text)
                length += CharWidth(c, scale);
            return length + System.Math.Max(0, (text.Length - 1) * Space * BaseScale * scale);
        }
    }
}