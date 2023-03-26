
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xylem.Component;
using Xylem.Registration;
using Xylem.Data;
using Xylem.Vectors;
using Xylem.Reference;

namespace Xylem.Graphics
{
    /**
     * This class contains references required for rendering that need only be
     * instantiated once, at the start of the program. 
     */
    public static class GraphicsContext
    {
        public static Source SourceInstance;
        public static GraphicsDeviceManager Graphics;
        public static GraphicsDevice Device => SourceInstance.GraphicsDevice;
        public static GraphicsAdapter Adapter => GraphicsAdapter.DefaultAdapter;
        public static SpriteBatch Renderer;

        public static int DisplayWidth => Adapter.CurrentDisplayMode.Width;
        public static int DisplayHeight => Adapter.CurrentDisplayMode.Height;

        public static int X => SourceInstance.Window.Position.X;
        public static int Y => SourceInstance.Window.Position.Y;

        public static GameWindow Window => SourceInstance.Window;

        internal static Dictionary<Color, Texture2D> PixelTextures = new Dictionary<Color, Texture2D>();

        /**
         * Provides a single-pixel texture of the supplied color. This is used elsewhere
         * to render shapes on a per-pixel basis.
         */
        public static Texture2D Pixel(Color color)
        {
            if (!PixelTextures.ContainsKey(color))
            {
                Texture2D pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
                pixel.SetData(new []{color});
                PixelTextures[color] = pixel;
            }

            return PixelTextures[color];
        }

        public static void Render(Texture2D texture, Rectangle area, Color? overlay = null)
        {
            Renderer.Draw(texture, area, overlay.GetValueOrDefault(Color.White));
        }

        public static void RenderPartial(Texture2D texture, Rectangle dest, Rectangle source)
        {
            Renderer.Draw(texture, dest, source, Color.White);
        }

        public static void StartRenderer() => Renderer = new SpriteBatch(Graphics.GraphicsDevice);
    }

    /**
     * This class contains utility functions for rendering textures and basic shapes.
     */
    public static class Pixel
    {
        public static Color DefaultColor { get; set; } = Color.White;

        /* Render a filled rectangle */

        public static void FillRect(int x, int y, int w, int h, Color color, Color? overlay = null) => FillRect(new Rectangle(x, y, w, h), color, overlay);

        public static void FillRect(int x, int y, int w, int h) => FillRect(new Rectangle(x, y, w, h), DefaultColor);

        public static void FillRect(Rectangle area) => FillRect(area, DefaultColor);

        public static void FillRect(Rectangle area, Color color, Color? overlay = null)
        {
            Texture2D pixel = GraphicsContext.Pixel(color);
            GraphicsContext.Render(pixel, area, overlay);
        }

        /* Render the frame of a rectangle */

        public static void FrameRect(int x, int y, int w, int h, Color color, Color? overlay = null) => FrameRect(new Rectangle(x, y, w, h), color, overlay);

        public static void FrameRect(int x, int y, int w, int h) => FrameRect(new Rectangle(x, y, w, h), DefaultColor);

        public static void FrameRect(Rectangle area, Color? overlay = null) => FrameRect(area, DefaultColor, overlay);

        public static void FrameRect(Rectangle area, Color color, Color? overlay = null, bool top = true, bool right = true, bool bottom = true, bool left = true)
        {
            Texture2D pixel = GraphicsContext.Pixel(color);
            if (top)
                GraphicsContext.Render(pixel, area.Top(), overlay);
            if (right)
                GraphicsContext.Render(pixel, area.Right(), overlay);
            if (bottom)
                GraphicsContext.Render(pixel, area.Bottom(), overlay);
            if (left)
                GraphicsContext.Render(pixel, area.Left(), overlay);
        }

        /* Recolors specific colored pixels in a texture to a specified replacement. */
        public static Texture2D Recolor(Texture2D texture, Color target, Color replacement)
        {
            Texture2D result = new Texture2D(GraphicsContext.Graphics.GraphicsDevice, texture.Width, texture.Height);

            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);

            for (int i = 0; i < colorData.Length; i ++)
            {
                if (colorData[i] == target)
                    colorData[i] = replacement;
            }

            result.SetData<Color>(colorData);

            return result;
        }

        public static Texture2D RecolorOpaque(Texture2D texture, Color replacement)
        {
            Texture2D result = new Texture2D(GraphicsContext.Graphics.GraphicsDevice, texture.Width, texture.Height);

            Color[] colorData = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colorData);

            for (int i = 0; i < colorData.Length; i ++)
                colorData[i] = replacement;

            result.SetData<Color>(colorData);

            return result;
        }
    }

    public static class RectangleExtensions
    {
        public static Rectangle Top(this Rectangle rect, int height = 1) => new Rectangle(rect.X, rect.Y, rect.Width, height);

        public static Rectangle Right(this Rectangle rect, int width = 1) => new Rectangle(rect.X + rect.Width - width, rect.Y, width, rect.Height);

        public static Rectangle Bottom(this Rectangle rect, int height = 1) => new Rectangle(rect.X, rect.Y + rect.Height - height, rect.Width, height);

        public static Rectangle Left(this Rectangle rect, int width = 1) => new Rectangle(rect.X, rect.Y, width, rect.Height);

        public static Rectangle RightSplit(this Rectangle rect, float portion)
        {
            Rectangle result = rect;
            result.Width = (int) (rect.Width * portion);
            result.X = rect.X + rect.Width - result.Width;

            return result;
        }

        public static Rectangle LeftSplit(this Rectangle rect, float portion)
        {
            Rectangle result = rect;
            result.Width = (int) (rect.Width * portion);
            return result;
        }

        public static Rectangle Grow(this Rectangle rect, int amount)
        {
            return new Rectangle(rect.X - amount, rect.Y - amount, rect.Width + (2 * amount), rect.Height + (2 * amount));
        }

        public static Rectangle Grow(this Rectangle rect, int top = 0, int right = 0, int bottom = 0, int left = 0)
        {
            return new Rectangle(rect.X - left, rect.Y - top, rect.Width + right + left, rect.Height + top + bottom);
        }

        public static Rectangle Shrink(this Rectangle rect, int amount)
        {
            return new Rectangle(rect.X + amount, rect.Y + amount, rect.Width - (2 * amount), rect.Height - (2 * amount));
        }

        public static Rectangle Shrink(this Rectangle rect, int top = 0, int right = 0, int bottom = 0, int left = 0)
        {
            return new Rectangle(rect.X + left, rect.Y + top, rect.Width - right - left, rect.Height - top - bottom);
        }

        public static Rectangle Shrink(this Rectangle rect, int ax, int ay)
        {
            return new Rectangle(rect.X + ax, rect.Y + ay, rect.Width - (2 * ax), rect.Height - (2 * ay));
        }

        public static Rectangle Move(this Rectangle rect, int ox, int oy)
        {
            return new Rectangle(rect.X + ox, rect.Y + oy, rect.Width, rect.Height);
        }

        public static Rectangle CenteredWithin(this Rectangle rect, float portionX = 1.0F, float portionY = 1.0F)
        {
            Rectangle result = rect;
            result.Width = (int) (rect.Width * portionX);
            result.Height = (int) (rect.Height * portionY);

            result.X = rect.X + (rect.Width - result.Width) / 2;
            result.Y = rect.Y + (rect.Height - result.Height) / 2;

            return result;
        }
    }

    public static class Text
    {
        public static void RenderTextWithin(string typeface, string text, Rectangle area, float scale = 1.0F) => RenderTextWithin(typeface, text, area, XFK.PrimaryText, scale);

        public static void RenderTextWithin(string typeface, string text, Rectangle area, Color color, float scale = 1.0F)
        {
            if (R.Typefaces.Has(typeface))
                RenderTextWithin(R.Typefaces[typeface], text, area, color, scale);
        }

        public static void RenderTextWithin(Typeface typeface, string text, Rectangle area, float scale = 1.0F) => RenderTextWithin(typeface, text, area, XFK.PrimaryText, scale);

        public static void RenderTextWithin(Typeface typeface, string text, Rectangle area, Color color, float scale = 1.0F, bool centerHorizontal = true)
        {
            // 1. Determine starting position (centered bidimensionally)

            float ax = area.X, ay = area.Y, aw = area.Width, ah = area.Height;

            float textWidth = typeface.StringWidth(text, scale);
            float textHeight = typeface.CharHeight(scale);

            int sx;

            if (centerHorizontal)
                sx = (int) (ax + (aw / 2) - textWidth / 2);
            else
                sx = (int) ax;

            int sy = (int) (ay + (ah / 2) - textHeight / 2);

            // 2. Render each character in the input string

            foreach (char c in text)
                sx += typeface.RenderChar(c, sx, sy, GraphicsContext.Renderer, color, scale);
        }

        public static void RenderTextAt(Typeface typeface, string text, Vec2i vec, float scale = 1.0F) => RenderTextAt(typeface, text, vec.X, vec.Y, XFK.PrimaryText, scale);

        public static void RenderTextAt(Typeface typeface, string text, int x, int y, float scale = 1.0F) => RenderTextAt(typeface, text, x, y, XFK.PrimaryText, scale);

        public static void RenderTextAt(Typeface typeface, string text, int x, int y, Color color, float scale = 1.0F, bool ignoreDebug = false)
        {
            foreach (char c in text)
                x += typeface.RenderChar(c, x, y, GraphicsContext.Renderer, color, scale, ignoreDebug);
        }
    }
}