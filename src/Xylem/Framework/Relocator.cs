
using System;
using Xylem.Input;
using Xylem.Graphics;
using Xylem.Functional;

namespace Xylem.Framework
{
    public abstract class Relocator 
    {
        public bool RelocateContainer { get; set; }

        public Provider<Frame> RelocationTarget { get; set; }

        public abstract void Relocate(Frame frame);
    }

    public class MouseDragRelocator : Relocator
    {
        public static MouseDragRelocator Left => new MouseDragRelocator(InputProcessor.MouseLeft);
        public static MouseDragRelocator Middle => new MouseDragRelocator(InputProcessor.MouseMiddle);
        public static MouseDragRelocator Right => new MouseDragRelocator(InputProcessor.MouseRight);

        public Function<Frame, int> LimitX { get; set; } = DefaultLimitX;
        public Function<Frame, int> LimitY { get; set; } = DefaultLimitY;

        protected readonly InputState MouseState;

        public MouseDragRelocator(InputState mouseState)
        {
            MouseState = mouseState;
        }

        public override void Relocate(Frame frame)
        {
            Frame basis = frame;

            if (RelocateContainer)
            {
                if (frame.Container != null)
                    basis = frame.Container;
                else
                    return;
            }

            else if (RelocationTarget != null)
            {
                basis = RelocationTarget();
            }

            if (InputProcessor.HasMouseMoved && (frame.Focused || frame.CursorWithin) && MouseState.Pressed)
            {
                basis.OX += InputProcessor.MDX;
                basis.OY += InputProcessor.MDY;

                // Lower and upper limits for offsets
                int llx, ulx;
                int lly, uly;

                if (basis.Container == null)
                {
                    llx = 0;
                    lly = 0;
                }

                else 
                {
                    llx = basis.Container.CX;
                    lly = basis.Container.CY;
                }

                ulx = LimitX(frame) - basis.RW;
                uly = LimitY(frame) - basis.RH;

                basis.OX = Math.Clamp(basis.OX, llx, ulx);
                basis.OY = Math.Clamp(basis.OY, lly, uly);

                FocusManager.SetFocused(frame);
            }

            if (MouseState.Released)
                FocusManager.SetUnfocused(frame);
        }

        public static int DefaultLimitX(Frame frame)
        {
            if (frame.Container == null)
                return GraphicsConfiguration.Width;
            else 
                return frame.Container.ContentArea.Right;
        }

        public static int DefaultLimitY(Frame frame)
        {
            if (frame.Container == null)
                return GraphicsConfiguration.Height;
            else 
                return frame.Container.ContentArea.Bottom;
        }
    }

    [Obsolete]
    public class MiddleDragRelocator : Relocator
    {
        [Obsolete]
        public static MiddleDragRelocator Instance = new MiddleDragRelocator();

        public Function<Frame, int> LimitX { get; set; } = DefaultLimitX;
        public Function<Frame, int> LimitY { get; set; } = DefaultLimitY;

        public override void Relocate(Frame frame)
        {
            if ((frame.Focused || frame.CursorWithin) && InputProcessor.MouseMiddle.Pressed)
            {
                frame.OX += InputProcessor.MDX;
                frame.OY += InputProcessor.MDY;

                // Lower and upper limits for offsets
                int llx, ulx;
                int lly, uly;

                if (frame.Container == null)
                {
                    llx = 0;
                    lly = 0;
                }

                else 
                {
                    llx = frame.Container.CX;
                    lly = frame.Container.CY;
                }

                ulx = LimitX(frame) - frame.RW;
                uly = LimitY(frame) - frame.RH;

                frame.OX = Math.Clamp(frame.OX, llx, ulx);
                frame.OY = Math.Clamp(frame.OY, lly, uly);

                FocusManager.SetFocused(frame);
            }
        }

        public static int DefaultLimitX(Frame frame)
        {
            if (frame.Container == null)
                return GraphicsConfiguration.Width;
            else 
                return frame.Container.ContentArea.Right;
        }

        public static int DefaultLimitY(Frame frame)
        {
            if (frame.Container == null)
                return GraphicsConfiguration.Height;
            else 
                return frame.Container.ContentArea.Bottom;
        }
    }
}