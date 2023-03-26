
using Microsoft.Xna.Framework;
using Xylem.Vectors;
using Xylem.Graphics;
using Xylem.Reference;

namespace Xylem.Framework
{
    public abstract class TooltipRenderer
    {
        public virtual void RenderAtCursor(Vec2i position, string text) => Render(position, text, 16, 16);

        public abstract void Render(Vec2i position, string text, int ox = 0, int oy = 0);

        public abstract int MeasureHeight();
    }

    public class XylemTooltipRenderer : TooltipRenderer
    {
        public static XylemTooltipRenderer Instance = new XylemTooltipRenderer();

        private XylemTooltipRenderer() {}

        public override int MeasureHeight() => XylemOptions.DefaultTypeface.ScaledHeight();

        public override void Render(Vec2i position, string text, int ox = 0, int oy = 0)
        {
            if (text.Length != 0)
            {
                int tw = (int) XylemOptions.DefaultTypeface.StringWidth(text);
                int th = (int) XylemOptions.DefaultTypeface.ScaledHeight();

                int rw = tw + 7, rh = th;

                int rx = position.X + ox, ry = position.Y + oy;

                if (GraphicsConfiguration.Width < rx + rw)
                    rx = position.X - ox - rw;

                Rectangle background = new Rectangle(rx, ry, rw, rh);

                Pixel.FillRect(background, XFK.PrimaryBorder);
                Pixel.FillRect(background, XFK.TooltipBackground);
                Pixel.FrameRect(background, XFK.TooltipBorder);

                Text.RenderTextWithin(XylemOptions.DefaultTypeface, text, background, XFK.TooltipText);
            }
        }
    }
}