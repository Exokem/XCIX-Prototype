
using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Xylem.Framework;
using Xylem.Input;
using Xylem.Functional;
using Xylem.Vectors;

namespace Vitreous.Framework.Game
{
    public abstract class MotiveFrame : Frame
    {
        // Temporary pan values
        protected virtual int TPX { get; set; } = 0;
        protected virtual int TPY { get; set; } = 0;

        // Aggregate pan values
        protected virtual int PX { get; set; } = 0;
        protected virtual int PY { get; set; } = 0;

        protected virtual IntRange PanBoundsHorizontal { get; set; } = IntRange.Arbitrary;
        protected virtual IntRange PanBoundsVertical { get; set; } = IntRange.Arbitrary;

        protected virtual int LowPanSpeed => 10;
        protected virtual int HighPanSpeed => 50;

        public override FocusUpdater FocusUpdater { get; set; } = new GainFocusOnAnyMouseInput();

        public MotiveFrame()
        {
            Focusable = true;
            Borderless = true;
        }

        protected override void UpdateFocusedInputs()
        {
            ProcessPanControl();
            ProcessZoomControl();
        }

        protected virtual void ProcessPanControl()
        {
            if (TPX != 0 || TPY != 0)
            {
                // Merge temporary pan values into aggregate
                PX += TPX;
                TPX = 0;
                PY += TPY;
                TPY = 0;
            }

            if (!Hovered)
                return;

            if (I.MIDDLE_MOUSE_PRESSED.RequestClaim())
            {
                if (InputProcessor.MDX != 0)
                    TPX += (PanBoundsHorizontal - PX).Clamp(InputProcessor.MDX);
                if (InputProcessor.MDY != 0)
                    TPY += (PanBoundsVertical - PY).Clamp(InputProcessor.MDY);
            }

            else if (I.SHIFT_MODIFIER.Query())
            {
                int rate = I.CONTROL_SHIFT_MODIFIER.Query() ? HighPanSpeed : LowPanSpeed;

                if (I.SCROLL_UP.RequestClaim())
                    PY += rate;
                else if (I.SCROLL_RIGHT.RequestClaim())
                    PX -= rate;
                else if (I.SCROLL_DOWN.RequestClaim())
                    PY -= rate;
                else if (I.SCROLL_LEFT.RequestClaim())
                    PX += rate;
            }

            PX = PanBoundsHorizontal.Clamp(PX);
            PY = PanBoundsVertical.Clamp(PY);
        }

        protected virtual void ProcessZoomControl() {}
    }

    public abstract class GridMotiveFrame : MotiveFrame
    {
        // Width and height of the grid in spaces
        public abstract int GridWidth { get; }
        public abstract int GridHeight { get; }

        // The base dimensions of each space
        public abstract int BaseSpaceWidth { get; }
        public abstract int BaseSpaceHeight { get; }

        // The current dimensions of each space
        public abstract int SpaceWidth { get; set; }
        public abstract int SpaceHeight { get; set; }

        protected Rectangle GridRenderArea => new Rectangle(GRX, GRY, GRW, GRH);

        public virtual int GRX => CX + PanBoundsHorizontal.Clamp(PX + TPX);
        public virtual int GRY => CY + PanBoundsVertical.Clamp(PY + TPY);
        protected virtual int GRW { get; set; }
        protected virtual int GRH { get; set; }

        protected override IntRange PanBoundsHorizontal => GetPanRange(0.125F, GRW, CW);
        protected override IntRange PanBoundsVertical => GetPanRange(0.125F, GRH, CH);

        protected virtual bool ShouldRenderOverlays => Hovered && SpaceHovered;

        public bool SpaceHovered { get; protected set; }

        // Hovered space grid x/y coordinates
        protected int HGX => Math.Clamp((InputProcessor.X - GRX) / SpaceWidth, 0, GridWidth - 1);
        protected int HGY => Math.Clamp((InputProcessor.Y - GRY) / SpaceHeight, 0, GridHeight - 1);

        // Hovered space rendered x/y coordinates
        protected int HX => HGX * SpaceWidth + GRX;
        protected int HY => HGY * SpaceHeight + GRY;

        protected override MouseCursor HoveredCursor
        {
            get 
            {
                if (InputProcessor.MouseMiddle.Pressed)
                    return MouseCursor.SizeAll;
                else 
                    return SpaceHovered ? MouseCursor.Crosshair : base.HoveredCursor;
            }
        }

        protected GridMotiveFrame()
        {
            // Initially position the grid render area within the content area
            UpdateDispatcher.EnqueueDelayedUpdate(() =>
            {
                if (GRW < CW)
                {
                    PX = (CW - GRW) / 2;
                    PY = (CH - GRH) / 2;
                }
                
                else 
                {
                    PX = BaseSpaceWidth;
                    PY = BaseSpaceHeight;
                }
            }, 10);
        }

        public override void Resize()
        {
            base.Resize();

            int spaceWidth = BaseSpaceWidth;
            int spaceHeight = BaseSpaceHeight;

            int scale = 2;

            while ((spaceWidth * scale * GridWidth) < CW && (spaceHeight * scale * GridHeight) < CH)
                scale ++;
            
            scale --;

            SpaceWidth = spaceWidth * scale;
            SpaceHeight = spaceHeight * scale;
        }

        protected IntRange GetPanRange(float minVisibleRatio, float tileRenderAreaDimension, float contentAreaDimension) 
        {
            float min, max;

            if (tileRenderAreaDimension < contentAreaDimension)
            {
                min = -((1 - minVisibleRatio) * tileRenderAreaDimension);
                max = contentAreaDimension - (minVisibleRatio * tileRenderAreaDimension);
            }

            else 
            {
                min = -(tileRenderAreaDimension - (minVisibleRatio * contentAreaDimension));
                max = (1 - minVisibleRatio) * contentAreaDimension;
            }

            return new IntRange((int) min, (int) max);
        }

        protected override void UpdateContentFrames()
        {
            GRW = SpaceWidth * GridWidth;
            GRH = SpaceHeight * GridHeight;

            SpaceHovered = Hovered && InputProcessor.CursorWithin(GridRenderArea);

            UpdateSpaces();
        }

        protected abstract void UpdateSpace(int x, int y);

        protected void UpdateSpaces()
        {
            for (int x = 0; x < GridWidth; x ++)
            {
                for (int y = 0; y < GridHeight; y ++)
                {
                    UpdateSpace(x, y);
                }
            }
        }

        protected override void RenderContentFrames()
        {
            RenderSpaces();
        }

        protected abstract void RenderSpace(int x, int y, Rectangle area);

        protected void RenderSpaces()
        {
            for (int x = 0; x < GridWidth; x ++)
            {
                for (int y = 0; y < GridHeight; y ++)
                {
                    Rectangle space = new Rectangle(GRX + (x * SpaceWidth), GRY + (y * SpaceHeight), SpaceWidth, SpaceHeight);
                    RenderSpace(x, y, space);
                }
            }
        }
        
        public Vec2i HoveredSpace()
        {
            int x = (InputProcessor.X - GridRenderArea.X) / SpaceWidth;
            int y = (InputProcessor.Y - GridRenderArea.Y) / SpaceHeight;

            x = Math.Clamp(x, 0, GridWidth - 1);
            y = Math.Clamp(y, 0, GridHeight - 1);

            return new Vec2i(x, y);
        }

        protected override void ProcessZoomControl()
        {
            if (!Hovered)
                return;

            if (I.SHIFT_MODIFIER.Query())
                return;

            bool up = I.SCROLL_UP.RequestClaim();
            bool down = I.SCROLL_DOWN.RequestClaim();

            if (!up && !down)
                return;

            float relMx = InputProcessor.X - GRX;
            float relMy = InputProcessor.Y - GRY;
            int prevTRW = GRW;
            int prevTRH = GRH;
            
            SpaceWidth += BaseSpaceWidth * (up ? 1 : -1);
            SpaceHeight += BaseSpaceHeight * (up ? 1 : -1);

            SpaceWidth = Math.Max(BaseSpaceWidth, SpaceWidth);
            SpaceHeight = Math.Max(BaseSpaceHeight, SpaceHeight);

            GRW = SpaceWidth * GridWidth;
            GRH = SpaceHeight * GridHeight;

            float scaleX = (float) GRW / (float) prevTRW;
            float scaleY = (float) GRH / (float) prevTRH;

            int newRelMx = (int) (relMx * scaleX);
            int newRelMy = (int) (relMy * scaleY);

            TPX -= newRelMx - (int) relMx;
            TPY -= newRelMy - (int) relMy;
        }
    }
}