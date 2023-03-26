
using System;
using Xylem.Input;
using Xylem.Functional;
using Xylem.Graphics;

namespace Xylem.Framework.Control
{
    public class ContextMenu : Frame
    {
        public Frame Source { get; protected set; }

        public bool IgnoreLayers { get; set; } = true;

        public delegate void ContextMenuAction(ContextMenu menu, Frame menuItem, Frame source);

        public ContextMenu()
        {
            Visibility = Visibilities.None;
            W = GraphicsConfiguration.Width / 9;
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();

            if (InputProcessor.AnyMouseClicked)
            {
                if (Visibility.ShouldRender() && !CursorWithin)
                {
                    Visibility = Visibilities.None;
                }
            }
        }

        public override void ContentActivated()
        {
            Visibility = Visibilities.None;
            // BlockingUpdater.UnsetContextMenu(this);
        }

        public override void Resize()
        {
            int h = 0;

            foreach (Frame frame in ContentFrames)
            {
                frame.Resize();
                frame.OX = 0;
                frame.OY = h;
                frame.W = Math.Max(CW, frame.W);
                W = Math.Max(W, frame.W);
                
                h += frame.H;
            }

            H = h + ContentInsets.Height;
        }

        public virtual void Reposition(Frame source)
        {
            Source = source;
            int mx = InputProcessor.X;
            int my = InputProcessor.Y;

            if (Container == null)
            {
                OX = mx;
                OY = my;
            }

            else 
            {
                OX = mx - Container.RX;
                OY = my - Container.RY;
            }

            OX += (GraphicsConfiguration.Width < RX + RW ? -RW : 0) - 1;
            OY += (GraphicsConfiguration.Height < RY + RH ? -RH : 0) - 1;
        }



        public ContextMenu Add(string label, Update action)
        {
            Add(new Button(label)
            {
                ActivationAction = v => action(),
                CenterText = false,
                ContentInsets = new Insets(5, 7, 5, 7)
            });

            return this;
        }

        public ContextMenu Add(string label, ContextMenuAction action)
        {
            Add(new Button(label)
            {
                ActivationAction = frame => action(this, frame, Source),
                CenterText = false,
                ContentInsets = new Insets(5, 7, 5, 7)
            });

            return this;
        }
    }
}