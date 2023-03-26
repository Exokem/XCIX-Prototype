
using Xylem.Graphics;
using Xylem.Functional;

namespace Xylem.Framework
{
    public abstract class Resizer
    {
        protected readonly Resizer Secondary;

        protected Resizer(Resizer secondary)
        {
            Secondary = secondary;
        }

        protected abstract void InternalResize(Frame frame, Frame basisFrame);

        public void Resize(Frame frame) => Resize(frame, frame.Container);

        public void Resize(Frame frame, Frame basisFrame)
        {
            InternalResize(frame, basisFrame);
            Secondary?.Resize(frame);
        }
    }

    public abstract class RatioResizer : Resizer
    {
        public static readonly Resizer ExpandBoth = new WidthRatio(secondary: new HeightRatio());
        public static readonly Resizer ExpandWidth = new WidthRatio();
        public static readonly Resizer ExpandHeight = new HeightRatio();

        public virtual float Ratio { get; set; } = 1F;

        protected RatioResizer(Resizer secondary = null) : base(secondary) {}

        protected abstract void RatioResize(Frame frame, int w, int h);

        protected sealed override void InternalResize(Frame frame, Frame basisFrame)
        {
            if (frame.Container != null)
                RatioResize(frame, frame.Container.RW, frame.Container.RH);
            else 
                RatioResize(frame, GraphicsConfiguration.Width, GraphicsConfiguration.Height);
        }
    }

    public sealed class SpecificResizer : Resizer
    {
        private readonly DualReceiver<Frame, Frame> Resizer;

        public SpecificResizer(DualReceiver<Frame, Frame> resizer) : base(null)
        {
            Resizer = resizer;
        }

        protected override void InternalResize(Frame frame, Frame basisFrame)
        {
            Resizer(frame, basisFrame);
        }
    }

    public class WidthRatio : RatioResizer
    {
        public WidthRatio(float ratio = 1F, Resizer secondary = null) : base(secondary)
        {
            Ratio = ratio;
        }

        protected override void RatioResize(Frame frame, int w, int h)
        {
            frame.W = (int) ((float) w * Ratio);
        }
    }

    public class HeightRatio : RatioResizer
    {
        public HeightRatio(float ratio = 1F, Resizer secondary = null) : base(secondary)
        {
            Ratio = ratio;
        }

        protected override void RatioResize(Frame frame, int w, int h)
        {
            frame.H = (int) ((float) h * Ratio);
        }
    }

    public class RoundHeightResizer : Resizer
    {
        public int Multiple { get; set; }
        public bool Truncate { get; set; }

        public RoundHeightResizer(int multiple, bool truncate = false, Resizer secondary = null) : base(secondary)
        {
            Multiple = multiple;
            Truncate = truncate;
        }

        protected override void InternalResize(Frame frame, Frame basisFrame)
        {
            if (Truncate)
                frame.H -= frame.H % Multiple;
            else if (frame.H % Multiple != 0)
                frame.H += Multiple - (frame.H % Multiple);
        }
    }
}