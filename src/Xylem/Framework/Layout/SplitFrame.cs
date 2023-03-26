
using System;

namespace Xylem.Framework.Layout
{
    public abstract class SplitFrame : Frame
    {
        public int Spacing { get; set; }

        protected bool ItemsChanged { get; set; }

        public abstract void AllocateSpace();

        public override void Add(Frame frame)
        {
            base.Add(frame);
            ItemsChanged = true;
        }

        public override void Resize()
        {
            base.Resize();
            
            AllocateSpace();
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            if (ItemsChanged)
                AllocateSpace();

            ItemsChanged = false;
        }
    }

    public class HorizontalSplitFrame : SplitFrame
    {
        public override int MeasuredHeight 
        {
            get 
            {
                int height = 0;

                foreach (Frame frame in ContentFrames)
                    height = Math.Max(height, frame.MeasuredHeight);

                return height;
            }
        }

        public override void AllocateSpace()
        {
            if (ContentFrames.Count == 0)
                return;

            int totalSpacing = Spacing * (ContentFrames.Count - 1);
            int itemWidth = (CW - totalSpacing) / ContentFrames.Count;

            // int totalAllocatedWidth = 0;

            int ox = 0;

            for (int i = 0; i < ContentFrames.Count; i++)
            {
                Frame frame = ContentFrames[i];
                frame.H = CH;
                frame.W = itemWidth;
                frame.OX = ox;
                
                if (ContentFrames.Count == i + 1)
                {
                    // This is the last frame, make sure all space is used
                    if (ox + itemWidth < CW)
                        frame.W = CW - ox;
                }

                ox += itemWidth + Spacing;
            }
        }

        protected override void RenderContentFrames()
        {
            Frame hoveredFrame = null;

            foreach (Frame frame in ContentFrames)
            {
                if (frame.Hovered && hoveredFrame == null)
                    hoveredFrame = frame;
                else 
                    frame.Render();
            }

            hoveredFrame?.Render();
        }
    }

    public class HorizontalSequenceFrame : HorizontalSplitFrame
    {
        public float Proportion { get; set; } = 0F;

        public override void AllocateSpace()
        {
            if (ContentFrames.Count == 0)
                return;

            int totalSpacing = Spacing * (ContentFrames.Count - 1);
            if (Proportion == 0)
                Proportion = 1F / (float) ContentFrames.Count;
            int itemWidth = (int) (Proportion * (float) (CW - totalSpacing));

            int ox = 0;

            foreach (Frame frame in ContentFrames)
            {
                frame.H = CH;
                frame.W = itemWidth;
                frame.OX = ox;
                ox += itemWidth + Spacing;
            }
        }
    }

    public class DynamicVerticalSplitFrame : SplitFrame
    {
        public override void AllocateSpace()
        {
            if (ContentFrames.Count == 0)
                return;

            int totalSpacing = Spacing * (ContentFrames.Count - 1);
            int availableSpace = CH - totalSpacing;

            foreach (Frame frame in ContentFrames)
            {
                if (!frame.ExpandVertical)
                    totalSpacing -= System.Math.Max(frame.RH, frame.MeasuredHeight);
            }

            int itemHeight = availableSpace / ContentFrames.Count;

            int oy = 0;

            foreach (Frame frame in ContentFrames)
            {
                frame.W = CW;
                frame.OY = oy;

                if (frame.ExpandVertical)
                {
                    frame.H = itemHeight;
                }

                oy += frame.H + Spacing;
            }
        }
    }

    public class VerticalSplitFrame : SplitFrame
    {
        public override void AllocateSpace()
        {
            if (ContentFrames.Count == 0)
                return;

            int totalSpacing = Spacing * (ContentFrames.Count - 1);
            int itemHeight = (CH - totalSpacing) / ContentFrames.Count;

            int oy = 0;

            foreach (Frame frame in ContentFrames)
            {
                frame.H = itemHeight;
                frame.W = CW;
                frame.OY = oy;
                oy += itemHeight + Spacing;
            }
        }

        protected override void RenderContentFrames()
        {
            Frame hoveredFrame = null;

            foreach (Frame frame in ContentFrames)
            {
                if (frame.Hovered && hoveredFrame == null)
                    hoveredFrame = frame;
                else 
                    frame.Render();
            }

            hoveredFrame?.Render();
        }
    }
}