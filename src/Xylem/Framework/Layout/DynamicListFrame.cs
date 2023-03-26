
using System;
using Microsoft.Xna.Framework;
using Xylem.Graphics;

namespace Xylem.Framework.Layout
{
    public class DynamicListFrame<V> : ListFrame<V>
    {
        public DynamicListFrame(string label = null) : base(label) {}

        public bool AutoSize { get; set; }

        public override void Resize()
        {
            base.Resize();

            if (ListLabel != null)
                ListLabel.W = CW;

            foreach (var item in ItemList)
                item.Frame.W = CW;

            if (!AutoSize)
            {
                base.Resize();
                return;
            }
            
            int totalItemHeight = 0;

            for (int i = 0; i < ItemList.Count; i ++)
            {
                Frame frame = ItemList[i].Frame;
                frame.H = System.Math.Max(frame.H, frame.MeasuredHeight);

                totalItemHeight += frame.H;
            }

            int extraSpace = RH - CH;
            H = totalItemHeight + LH + extraSpace;
        }

        protected override void UpdateContentFrames()
        {
            int totalOffset = LH;
            // int heightRendered = (CY - RY - BY) + LH;

            ListLabel?.Update();

            int i = StartIndex;

            for (; i < ItemList.Count; i ++)
            {
                Frame frame = ItemList[i].Frame;
                
                if (CH < totalOffset + frame.RH)
                    break;

                frame.OX = ItemOffset;
                frame.OY = totalOffset;
                frame.W = CW;
                frame.Update();
                    
                totalOffset += frame.RH;
            }

            DisplayedItems = Math.Min(i - StartIndex, ItemList.Count);
        }

        protected override void RenderContentFrames()
        {
            // if (GraphicsConfiguration.Debug)
            //     Pixel.FillRect(RenderArea, Color.Red);

            ListLabel?.Render();

            for (int i = StartIndex; i < StartIndex + DisplayedItems; i ++)
            {
                IListItem<V> item = ItemList[i];
                item.Frame.Render();
            }
        }

        public void Clear()
        {
            ItemList.Clear();
            ContentFrames.Clear();
        }
    }
}