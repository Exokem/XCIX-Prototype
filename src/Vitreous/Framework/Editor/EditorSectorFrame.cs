
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Vectors;
using Xylem.Framework;
using Xylem.Functional;
using Vitreous.Framework.Game;
using Vitreous.Component.Spatial;

namespace Vitreous.Framework.Editor
{
    public class EditorSectorFrame : SectorFrame, IToolFrame<Sector>
    {
        readonly IToolFrame<Sector> ToolFrame;

        public IEditorTool<Sector> ActiveTool { get; set; }

        public Vec2i TargetPosition => HoveredSpace();

        public Sector Component 
        { 
            get => SectorComponent; 
            set { SectorComponent = value; }
        }

        public Stack<ReversibleAction> UndoStack { get; } = new Stack<ReversibleAction>();
        public Stack<ReversibleAction> RedoStack { get; } = new Stack<ReversibleAction>();

        public EditorSectorFrame()
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

        internal virtual void RenderRulers()
        {
            EditorRulers.RenderRulers(this, Sector.GridWidth, Sector.GridHeight, GRX, GRY, SpaceWidth, SpaceHeight);
        }
    }
}