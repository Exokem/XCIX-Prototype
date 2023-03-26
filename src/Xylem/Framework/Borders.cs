
using Microsoft.Xna.Framework;
using Xylem.Graphics;
using Xylem.Reference;

namespace Xylem.Framework
{
    public abstract class Border
    {
        public bool Inset { get; set; } = true;

        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }

        public abstract void Render(Rectangle area);

        public abstract void RenderInset(Rectangle area);

        public virtual int Horizontal() => Right + Left;
        public virtual int Vertical() => Top + Bottom;
    }

    public class SimpleBorder : Border
    {
        // public bool Inset { get; set; }

        // public int Top { get; set; }
        // public int Right { get; set; }
        // public int Bottom { get; set; }
        // public int Left { get; set; }

        public Color Color { get; set; }

        public SimpleBorder(int top, int right, int bottom, int left, Color? color = null)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;

            Color = color.GetValueOrDefault(XFK.PrimaryBorder);
        }

        public SimpleBorder(int width, Color? color = null) : this(width, width, width, width, color) {}

        public SimpleBorder(int horizontal, int vertical, Color? color = null) : this (vertical, horizontal, vertical, horizontal, color) {}

        public SimpleBorder(Color? color = null) : this(1, color) {}

        public override void Render(Rectangle area)
        {
            if (Inset)
            {
                RenderInset(area);
            }

            else
            {
                Rectangle top = area.Top(0).Grow(left: Left, right: Right, top: Top);
                Pixel.FillRect(top, Color);

                Rectangle right = area.Right(0).Grow(right: Right);
                Pixel.FillRect(right, Color);

                Rectangle bottom = area.Bottom(0).Grow(left: Left, right: Right, bottom: Bottom);
                Pixel.FillRect(bottom, Color);

                Rectangle left = area.Left(0).Grow(left: Left);
                Pixel.FillRect(left, Color);
            }
        }

        public override void RenderInset(Rectangle area)
        {
            Rectangle top = area.Top();
            top.Height = Top;
            Pixel.FillRect(top, Color);

            Rectangle right = area.Right();
            right.Offset(1 - Right, 0);
            right.Width = Right;
            Pixel.FillRect(right, Color);

            Rectangle bottom = area.Bottom();
            bottom.Offset(0, 1 - Bottom);
            bottom.Height = Bottom;
            Pixel.FillRect(bottom, Color);

            Rectangle left = area.Left();
            left.Width = Left;
            Pixel.FillRect(left, Color);            
        }
    }
}