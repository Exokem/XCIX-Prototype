
using System;

namespace Xylem.Framework.Layout
{
    public class DirectionalFrame : Frame
    {
        public int ContentSpace { get; set; } = 0;
        protected int Expanders { get; private set; } = 0;
        public bool Vertical { get; private set; }
        public bool Measure { get; set; } = false;

        protected int FlexibleSpace = 0;

        private bool _itemsChanged;

        public DirectionalFrame(bool vertical = true)
        {
            Vertical = vertical;
        }

        public sealed override void Add(Frame frame)
        {
            base.Add(frame);
            frame.OX = frame.OY = 0;

            if ((Vertical && frame.ExpandVertical) || (!Vertical && frame.ExpandHorizontal))
                Expanders ++;

            if (frame.Border != null)
            {
                if (Vertical)   
                    frame.Border.Top = 0;
                else 
                    frame.Border.Left = 0;
            }

            _itemsChanged = true;
        }

        public override void Resize()
        {
            FlexibleSpace = Vertical ? CH : CW;

            int maxDim = 0;

            foreach (Frame frame in ContentFrames)
            {
                if (Measure)
                {
                    if (frame.W == 0)
                        frame.W = frame.MeasuredWidth;
                    if (frame.H == 0)
                        frame.H = frame.MeasuredHeight;
                }

                if (Vertical && !frame.ExpandVertical)
                    FlexibleSpace -= frame.RH;
                else if (!Vertical && !frame.ExpandHorizontal)
                    FlexibleSpace -= frame.RW;
                
                maxDim = System.Math.Max(maxDim, (Vertical ? frame.W : frame.H));
            }

            if (H == 0 && !Vertical)
                H = maxDim;
            else if (W == 0 && Vertical)
                W = maxDim;

            FlexibleSpace -= ContentSpace * (ContentFrames.Count - 1);

            base.Resize();
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            if (_itemsChanged)
            {
                Resize();
                _itemsChanged = false;
            }
        }

        protected override void RenderContentFrames()
        {
            int offset = 0;
            foreach (Frame contentFrame in ContentFrames)
            {
                if (Vertical)
                {
                    if (contentFrame.ExpandVertical)
                        contentFrame.H = FlexibleSpace / Expanders;

                    contentFrame.OY = offset;
                    contentFrame.W = CW;
                    offset += ContentSpace + contentFrame.RH;
                }

                else 
                {
                    if (contentFrame.ExpandHorizontal)
                        contentFrame.W = FlexibleSpace / Expanders;

                    contentFrame.OX = offset;
                    contentFrame.H = CH;
                    offset += ContentSpace + contentFrame.RW;
                }

                contentFrame.Render();
            }
        }
    }
}