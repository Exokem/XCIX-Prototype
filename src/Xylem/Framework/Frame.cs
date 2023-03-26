
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Xylem.Graphics;
using Xylem.Input;
using Xylem.Data;
using Xylem.Functional;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Vectors;
using Xylem.Reference;

namespace Xylem.Framework
{
    public class Frame
    {
        /* The container of this frame. */
        private Frame _container;
        public Frame Container 
        {
            get => _container;
            set 
            {
                _container = value;
                if (value == null)
                    _renderIndex = 0;
            }
        }

        public int ContainerHeight => Container == null ? GraphicsConfiguration.Height : Container.CH;
        public int ContainerWidth => Container == null ? GraphicsConfiguration.Width : Container.CW;

        // internal RenderIndexTracker IndexTracker = new RenderIndexTracker();
        internal int _maxHoveredIndex = 0;
        internal int MaxHoveredIndex 
        {
            get => Container == null ? _maxHoveredIndex : Container.MaxHoveredIndex;
            set
            {
                if (Container == null)
                    _maxHoveredIndex = value;
                else 
                    Container.MaxHoveredIndex = value;
            }
        }

        /* The frames that this frame contains. */
        internal List<Frame> ContentFrames = new List<Frame>();

        /* The insets that offset content rendered within this frame. */
        public virtual Insets ContentInsets { get; set; } = new Insets(0);

        public virtual Insets ContentMargin { get; set; } = new Insets(0);

        /* The offsets for the position of this frame. */
        public virtual int OX { get; set; }
        public virtual int OY { get; set; }

        public virtual Relocator Relocator { get; set; } = null;

        public bool OffsetContent { get; set; }

        /* The default position of this frame. */
        public int X => Container == null ? OX : Container.X + Container.ContentInsets.Left + OX;
        public int Y => Container == null ? OY : Container.Y + Container.ContentInsets.Top + OY;

        internal bool DimensionChanged = true;

        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public virtual Resizer Resizer { get; set; }

        public virtual void Resize() 
        {
            Resizer?.Resize(this);
            foreach (Frame frame in ContentFrames)
                frame.Resize();
        }

        private int _w, _h;

        /* The width and height of this frame. */
        public int W
        {
            get => _w; 
            set 
            {
                if (_w != value)
                    DimensionChanged = true;

                _w = value;
            }
        }

        public int H
        {
            get => _h;
            set
            {
                if (_h != value)
                    DimensionChanged = true;
                _h = value;
            }
        }

        /* The actual rendered position and dimensions of this frame. */
        public int RX => RenderArea.X;
        public int RY => RenderArea.Y;
        public int RW => RenderArea.Width;
        public int RH => RenderArea.Height;

        public virtual int MeasuredWidth => ContentInsets.Width + ContentMargin.Width;
        public virtual int MeasuredHeight => ContentInsets.Height + ContentMargin.Height;

        /* Provides a rectangle representing the area occupied by this frame. */
        public Rectangle SpatialArea => new Rectangle(X, Y, W, H);

        /* The actual rendered position and dimensions of the content area. */
        public virtual int CX => RX + ContentInsets.Left + ContentMargin.Left + BX;
        public virtual int CY => RY + ContentInsets.Top + ContentMargin.Top + BY;
        public virtual int CW => RW - ContentInsets.Width - ContentMargin.Width - BW;
        public virtual int CH => RH - ContentInsets.Height - ContentMargin.Height - BH;

        private Border _border = new SimpleBorder(1);
        public virtual Border Border
        {
            get => _border;
            set
            {
                if (value != null)
                    _border = value;
            }
        }

        // Border Offsets
        public int BX => 0;
        public int BY => 0;
        public int BW => 0;
        public int BH => 0;

        public Rectangle ContentArea => new Rectangle(CX, CY, CW, CH);

        public virtual int MX => CX - ContentMargin.Left;
        public virtual int MY => CY - ContentMargin.Top;
        public virtual int MW => CW + ContentMargin.Width;
        public virtual int MH => CH + ContentMargin.Height;

        public Rectangle MarginArea => new Rectangle(MX, MY, MW, MH);

        /* This is an optional override to the area a frame is rendered in. */
        protected Rectangle _renderArea;
        public Rectangle RenderArea
        {
            get 
            {
                if (!Visibility.ShouldOccupySpace())
                    return new Rectangle(0, 0, 0, 0);
                else 
                    return _renderArea.Width == 0 && _renderArea.Height == 0 ? SpatialArea : _renderArea;
            }
            set => _renderArea = value;
        }

        /* The primary color of this frame. */
        public virtual Color PrimaryColor { get; set; } = XFK.PrimaryBorder;
        public virtual Color BackgroundColor { get; set; } = Color.Transparent;

        // Which cursor should be used when this frame is hovered
        protected virtual MouseCursor HoveredCursor { get; set; } = MouseCursor.Arrow;

        public bool CursorWithin { get; private set; }
        public readonly Property<bool> HoveredProperty = new Property<bool>(false);
        private DateTime _hoverStart;
        protected double HoveredSeconds => Hovered ? (DateTime.Now - _hoverStart).TotalSeconds : 0D;
        public bool Hovered
        {
            get => HoveredProperty.Value;
            set 
            {
                if (!HoveredProperty.Value && value)
                    _hoverStart = DateTime.Now;
                
                HoveredProperty.Value = value;
            }
        }

        public virtual TooltipRenderer TooltipRenderer { get; set; } = XylemTooltipRenderer.Instance;

        public virtual FocusUpdater FocusUpdater { get; set; } = new StandardMouseLeft();

        public readonly Property<bool> FocusedProperty = new Property<bool>(false);
        public bool Focused 
        {
            get => FocusedProperty.Value;
            set => FocusedProperty.Value = value;
        }

        public readonly Property<bool> ActivatedProperty = new Property<bool>(false);
        public bool Activated 
        {
            get => ActivatedProperty.Value;
            set => ActivatedProperty.Value = value;
        }

        public bool JustActivated { get; private set; }

        public bool Borderless { get; set; } = false;

        // Can this frame be focused - enable/disable auto focus and input managing 
        public bool Focusable { get; set; } = false;

        public bool ExpandHorizontal { get; set; } = false;
        public bool ExpandVertical { get; set; } = false;

        public int SpanColumns { get; set; } = 1;
        public int SpanRows { get; set; } = 1;

        public virtual void ContentActivated() {}

        protected int _renderIndex { get; set; } = 0;
        private bool _renderIndexOverridden = false;
        public int RenderIndex
        {
            get => _renderIndex;
            set
            {
                _renderIndexOverridden = true;
                _renderIndex = value;
            }
        }

        // This allows frames to request a render priority
        // Whether it is effective depends on the container frame
        // Higher priorities should indicate that this frame will be rendered sooner
        public int RenderPriority { get; set; }

        public IVisibility Visibility { get; set; } = Visibilities.Visible;

        public delegate void FrameAction(Frame frame);
        public FrameAction ActivationAction;

        public string TooltipText { get; set; } = "";

        private ContextMenu _contextMenu = null;
        public ContextMenu ContextMenu { get => _contextMenu; set => SetContextMenu(value); }

        public virtual void SetContextMenu(ContextMenu menu)
        {
            _contextMenu = menu;
            _contextMenu.Container = this;
            _contextMenu.RenderIndex = 99;
            // Add(ContextMenu);
        }

        /* Add a frame to the content body of this frame. */
        public virtual void Add(Frame frame)
        {
            if (frame == null)
                return;

            ContentFrames.Add(frame);
            frame.Container = this;
        } 

        public void AddAll(params Frame[] frames)
        {
            foreach (Frame frame in frames)
                Add(frame);
        }

        public virtual void Remove(Frame frame)
        {
            ContentFrames.Remove(frame);
            frame.Container = null;
        } 

        /** 
         * Virtual update functions for functionality before and after the required 
         * default update logic occurs. 
         */
        protected virtual void UpdateContentFrames() 
        {
            foreach (var frame in ContentFrames)
            {
                frame.Update();
            }
        }

        protected virtual void PostUpdate() {}

        protected virtual void OnDimensionChanged()
        {
            foreach (Frame frame in ContentFrames)
                frame.OnDimensionChanged();
        }

        protected virtual void OnActivated() {}

        protected virtual void UpdateFocusedInputs() {}
        protected virtual void UpdateGlobalInputs() {}

        private void UpdateRenderIndices()
        {
            // If this is a root frame, reset the max index
            // This happens before updating content frames because they will try to 
            // adjust the max index
            if (Container == null)
                MaxHoveredIndex = 0;

            // Update render index internally if it has not been overridden
            if (!_renderIndexOverridden)
            {
                _renderIndex = Container == null ? 0 : Container.RenderIndex + 1;
            }

            foreach (Frame frame in ContentFrames)
            {
                if (frame.Visibility.ShouldUpdate())
                    frame.UpdateRenderIndices();
            }
            
            CursorWithin = InputProcessor.CursorWithin(SpatialArea);

            // Compare render index to current maximum if the cursor is within this frame
            if (CursorWithin)
                MaxHoveredIndex = System.Math.Max(MaxHoveredIndex, RenderIndex);
        }

        /* Update the state attributes of this frame. */
        public void Update()
        {
            if (!Visibility.ShouldUpdate())
                return;

            if (W == 0)
                W = MeasuredWidth;
            if (H == 0)
                H = MeasuredHeight;    

            // Calculate all render indices
            // Determine which frames contain the cursor
            // Find the maximum index of a frame that also contains the cursor
            UpdateRenderIndices();

            if (ContextMenu != null && ContextMenu.Visibility.ShouldRender())
                MaxHoveredIndex = ContextMenu.RenderIndex;

            // Update Frames
            // This happens after updating indices so frames with overridden indices
            // will not have their indices disregarded by the component update order
            UpdateContentFrames();

            bool shouldHover = CursorWithin && _renderIndex == MaxHoveredIndex;

            if (HoveredProperty.Value != CursorWithin || Hovered != shouldHover)
            {
                Hovered = shouldHover;
            }

            // Keep this after content frames are resized so they can know when their 
            // container has been resized
            if (DimensionChanged)
            {
                OnDimensionChanged();

                if (Container == null)
                {
                    foreach (Frame frame in ContentFrames)
                        frame.Resize();
                }
                
                DimensionChanged = false;
            }

            else if (Container != null && Container.DimensionChanged)
            {
                Resizer?.Resize(this);
            }

            HorizontalAlignment?.Align(this);
            VerticalAlignment?.Align(this);

            if (Focusable)
            {
                FocusUpdater?.UpdateFocus(this);
                if (Focused)
                    UpdateFocusedInputs();
            }

            Relocator?.Relocate(this);

            UpdateGlobalInputs();

            if (Hovered)
            {
                CursorStateManager.RequestCursor(HoveredCursor);

                if (Activated)
                    JustActivated = false;

                // If the frame is not activated and the mouse is clicked within it,
                // activate the frame. 
                if (!Activated && InputProcessor.MouseLeft.Clicked)
                {
                    JustActivated = true;
                    Activated = true;
                    OnActivated();
                    ActivationAction?.Invoke(this);

                    Container?.ContentActivated();
                }

                // If the frame is activated and the mouse is released within it,
                // deactivate the frame.
                else if (Activated && InputProcessor.MouseLeft.Released)
                    Activated = false;
            }
            // Deactivate the frame as soon as the mouse leaves its area, regardless
            // of whether or not the mouse button is still held.
            else Activated = false;

            ContextMenu?.Update();

            if (ContextMenu != null && InputProcessor.MouseRight.Clicked)
            {
                bool shouldDisplay = ContextMenu.IgnoreLayers ? CursorWithin : Hovered;
                
                if (shouldDisplay)
                {
                    UpdateDispatcher.EnqueueUpdate(() => 
                    {
                        ContextMenu.Visibility = Visibilities.Visible;
                        ContextMenu.Reposition(this);
                        ContextMenu.Resize();
                    });
                    // BlockingUpdater.SetContextMenu(ContextMenu);
                }
            }

            PostUpdate();
        }

        protected virtual void RenderBackground()
        {
            if (BackgroundColor != Color.Transparent)
                Pixel.FillRect(RenderArea, BackgroundColor);
        }

        /* Render the content of this frame. */
        protected virtual void RenderContentFrames()
        {
            foreach (var frame in ContentFrames)
                frame.Render();
        }

        protected virtual void RenderExtraContent() {}

        private readonly Queue<Update> _preOverlayRenderActions = new Queue<Update>();

        public virtual void InjectPreOverlayRenderAction(Update renderAction) => _preOverlayRenderActions.Enqueue(renderAction);

        protected virtual void RenderOverlays()
        {

        }

        protected virtual void RenderDebugOverlays()
        {

        }

        /* Render this frame. */
        public void Render()
        {
            if (!Visibility.ShouldRender())
                return;

            RenderBackground();

            // if (GraphicsConfiguration.Debug && Focused)
            //     Pixel.FillRect(RenderArea, Color.LightCoral);

            RenderExtraContent();
            RenderContentFrames();

            if (!Borderless)
            {
                Border.Render(RenderArea);

                if (GraphicsConfiguration.Debug && CursorWithin)
                {
                    if (Border is SimpleBorder simple)
                    {
                        Border debugBorder = new SimpleBorder(1, Color.Green);
                        UpdateDispatcher.EnqueueRenderAction(() => debugBorder.Render(RenderArea));
                    }
                }
                // RenderBorder();
            }
            

            while (_preOverlayRenderActions.Count != 0)
                _preOverlayRenderActions.Dequeue()();

            RenderOverlays();

            if (GraphicsConfiguration.Debug)
            {
                RenderDebugOverlays();
                if (Hovered)
                    Text.RenderTextWithin(XylemOptions.DefaultTypeface, $"{GetType()}", RenderArea, Color.White);
            }

            if (GraphicsConfiguration.Debug && this is HorizontalSplitFrame)
            {
                Pixel.FrameRect(RenderArea, Color.Yellow);
                Pixel.FrameRect(MarginArea, Color.Purple);
                Pixel.FrameRect(ContentArea, Color.Green);

                Text.RenderTextWithin(XylemOptions.DefaultTypeface, $"{_renderIndex}/{MaxHoveredIndex}", RenderArea.RightSplit(0.3F), Color.Purple);

                

                string debugText = $"({RX},{RY}) : {RW}x{RH}";

                // if (CursorWithin && this is DynamicListFrame<Frame>)
                //     Text.RenderTextAt(Options.InterfaceTypeface, debugText, RX - (int) Options.InterfaceTypeface.StringWidth(debugText), RY + RH / 2);
            }

            if (ContextMenu != null && ContextMenu.Visibility.ShouldRender())
                UpdateDispatcher.EnqueueRenderAction(ContextMenu.Render);

            if (Hovered && InputProcessor.Pressed(Shortcuts.UIShowTooltips))
                UpdateDispatcher.EnqueueRenderAction(() => TooltipRenderer.RenderAtCursor(new Vec2i(InputProcessor.X, InputProcessor.Y), TooltipText));
            if (0.7D < HoveredSeconds)
                UpdateDispatcher.EnqueueRenderAction(() => TooltipRenderer.RenderAtCursor(new Vec2i(InputProcessor.X, InputProcessor.Y), TooltipText));
        }

        /* Extra functionality. */
        public virtual void ReceiveTextInput(TextInputEventArgs args) {}
    }

    public struct Insets
    {
        public int Top, Right, Bottom, Left;

        public int Width => Right + Left;
        public int Height => Top + Bottom;

        public Insets(int top = 0, int right = 0, int bottom = 0, int left = 0)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public Insets(int space)
        {
            Top = Right = Bottom = Left = space;
        }

        public Insets(int horizontal = 0, int vertical = 0)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }
    }
}