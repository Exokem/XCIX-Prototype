
using Microsoft.Xna.Framework;
using Xylem.Graphics;

namespace Xylem.Framework
{
    public abstract class Alignment
    {
        public int Offset { get; set; } = 0;

        protected Alignment()
        {
        }

        public abstract void Align(Frame frame);
    }

    public abstract class HorizontalAlignment : Alignment
    {

    }

    public class RightAlignment : HorizontalAlignment
    {
        public override void Align(Frame frame)
        {
            if (frame.Container != null)
                frame.OX = frame.Container.RenderArea.Right - frame.RW;
            else
                frame.OX = GraphicsConfiguration.Width - frame.RW;
        }
    }

    public class LeftAlignment : HorizontalAlignment
    {
        public override void Align(Frame frame)
        {
            if (frame.Container != null)
                frame.OX = frame.Container.RenderArea.Left;
            else
                frame.OX = 0;
        }
    }

    public class CenteredHorizontalAlignment : HorizontalAlignment
    {
        public static readonly CenteredHorizontalAlignment Instance = new CenteredHorizontalAlignment();

        public override void Align(Frame frame)
        {
            if (frame.Container != null)
                frame.OX = (frame.Container.RW - frame.RW) / 2;
            else 
                frame.OX = (GraphicsConfiguration.Width - frame.RW) / 2;
        }
    }

    public abstract class VerticalAlignment : Alignment
    {

    }

    public class CenteredVerticalAlignment : VerticalAlignment
    {
        public static readonly CenteredVerticalAlignment Instance = new CenteredVerticalAlignment();

        public override void Align(Frame frame)
        {
            if (frame.Container != null)
                frame.OY = (frame.Container.RH - frame.RH) / 2;
            else 
                frame.OY = (GraphicsConfiguration.Height - frame.RH) / 2;
        }
    }

    public class BottomAlignment : VerticalAlignment
    {
        public override void Align(Frame frame)
        {
            if (frame.Container != null)
                frame.OY = frame.Container.RH - frame.RH - Offset;
            else 
                frame.OY = GraphicsConfiguration.Height - frame.RH - Offset;
        }
    }
}