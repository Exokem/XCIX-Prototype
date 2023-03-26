
using System;
using Xylem.Graphics;
using Xylem.Input;
using Xylem.Functional;

namespace Xylem.Framework.Control
{
    public class Menu : Button
    {
        public bool ShowItems { get; set; }

        protected bool AnyItemsHovered;

        public Menu(string label = "") : base(label) {}

        // Description
        //
        // Menu is clicked: becomes open
        // Click occurs outside of menu: becomes closed

        protected bool Open { get; set; }

        protected override void UpdateContentFrames() 
        {
            if (Open)
            {
                base.UpdateContentFrames();
            }
        }

        protected override void OnActivated()
        {
            Open = !Open;

            foreach (var frame in ContentFrames)
                frame.Visibility = Open ? Visibilities.Visible : Visibilities.None;
        }

        protected override void PostUpdate()
        {
            if (!CursorWithin && InputProcessor.MouseLeft.Clicked)
            {
                Open = false;

                foreach (var frame in ContentFrames)
                    frame.Visibility = Visibilities.None;
            }
        }

        public override void Resize()
        {
            int y = RH - ContentInsets.Top;

            foreach (var frame in ContentFrames)
            {
                frame.W = RW;
                frame.H = frame.MeasuredHeight;
                frame.OY = y;
                frame.OX = -ContentInsets.Left;
                y += frame.RH;
            }

            base.Resize();
        }

        public override void Add(Frame frame)
        {
            base.Add(frame);
            frame.Border.Top = 0;
            if (!Open)
                frame.Visibility = Visibilities.None;
        }

        protected override void RenderContentFrames()
        {
            if (Open)
            {
                foreach (Frame frame in ContentFrames)
                    frame.W = CW;

                UpdateDispatcher.EnqueueRenderAction(base.RenderContentFrames);
            }
        }
    }

    public class NestedMenu : Menu
    {
        public override void Resize()
        {
            int y = -ContentInsets.Top;

            foreach (var frame in ContentFrames)
            {
                frame.W = CW;
                frame.H = frame.MeasuredHeight;
                frame.OX = W;
                frame.OY = y;
                y += frame.RH;
            }
        }
    }
}