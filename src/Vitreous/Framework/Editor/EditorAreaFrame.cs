
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Functional;
using Xylem.Graphics;
using Xylem.Vectors;
using Xylem.Registration;
using Vitreous.Component.Composite;
using Vitreous.Framework.Game;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    public class EditorAreaFrame : AreaFrame, IToolFrame<Area>
    {
        readonly IToolFrame<Area> ToolFrame;

        // private IEditorTool<Area> _activeTool;
        public IEditorTool<Area> ActiveTool { get; set; }
        // { 
        //     get => _activeTool; 
        //     set
        //     {
        //         if (value != null && (_activeTool == null || !_activeTool.EquivalentTo(value)))
        //             _activeTool = value;
        //     }
        // }

        public Vec2i TargetPosition => HoveredSpace();

        public Area Component 
        { 
            get => AreaComponent; 
            set { AreaComponent = value; }
        }

        public Stack<ReversibleAction> UndoStack { get; } = new Stack<ReversibleAction>();
        public Stack<ReversibleAction> RedoStack { get; } = new Stack<ReversibleAction>();

        public EditorAreaFrame() : base()
        {
            ToolFrame = this;
        }

        protected override void UpdateContentFrames()
        {
            base.UpdateContentFrames();

            if (SpaceHovered)
                ToolFrame.UpdateActiveTool();
        }

        protected override void UpdateFocusedInputs()
        {
            ToolFrame.UpdateFocusedToolInputs();

            base.UpdateFocusedInputs();
        }

        protected override void RenderOverlays()
        {   
            if (ShouldRenderOverlays && !ToolFrame.RenderToolOverlays(new Rectangle(HX, HY, SpaceWidth, SpaceHeight)))
                base.RenderOverlays();

            RenderRulers();
        }

        protected override void RenderDebugOverlays()
        {
            base.RenderDebugOverlays();

            int dx = RX + 24 + 10;
            int dy = RY + 24 + 10;
            int iy = 0;

            Text.RenderTextAt(R.Typefaces["rowui"], $"Panned Offsets: {PX}, {PY}", dx, dy + (iy++ * 30));
            Text.RenderTextAt(R.Typefaces["rowui"], $"Tilemap Position: {GRX}, {GRY}", dx, dy + (iy++ * 30));
            Text.RenderTextAt(R.Typefaces["rowui"], $"Tilemap Dimensions: {GRW}, {GRH}", dx, dy + (iy++ * 30));
            Text.RenderTextAt(R.Typefaces["rowui"], $"Temp Panned Offsets: {TPX}, {TPY}", dx, dy + (iy++ * 30));
            Text.RenderTextAt(R.Typefaces["rowui"], $"Viewport Dimensions: {CW}, {CH}", dx, dy + (iy++ * 30));
        }

        internal virtual void RenderRulers()
        {
            EditorRulers.RenderRulers(this, Area.GridWidth, Area.GridHeight, GRX, GRY, SpaceWidth, SpaceHeight);
        }

        private void SetStructure(Vec2i v, Structure structure)
        {
            AreaComponent.SetStructure(v, structure);
        }
    }
}