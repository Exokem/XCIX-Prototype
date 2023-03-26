
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Data;
using Xylem.Graphics;
using Xylem.Input;
using Xylem.Reference;
using Xylem.Framework;
using Xylem.Vectors;
using Xylem.Functional;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Game
{
    public class AreaFrame : GridMotiveFrame
    {
        public Area AreaComponent;

        public override int GridWidth => Area.GridWidth;
        public override int GridHeight => Area.GridHeight;

        public override int BaseSpaceWidth => SpatialOptions.TileWidth;
        public override int BaseSpaceHeight => SpatialOptions.TileHeight;

        public override int SpaceWidth { get; set; } = SpatialOptions.TileWidth;
        public override int SpaceHeight { get; set; } = SpatialOptions.TileHeight;

        public virtual Color AreaBackgroundColor { get; set; } = VFK.EditorAreaBackground;
        public override Color BackgroundColor { get; set; } = VFK.EditorViewportBackground;

        // protected override MouseCursor HoveredCursor
        // {
        //     get 
        //     {
        //         if (InputProcessor.MouseMiddle.Pressed)
        //             return MouseCursor.SizeAll;
        //         else 
        //             return SpaceHovered ? MouseCursor.Crosshair : base.HoveredCursor;
        //     }
        // }

        protected override void UpdateSpace(int x, int y)
        {
            if (AreaComponent == null)
                return;

            Tile tile = AreaComponent[x, y];

            if (tile.Invalidated)
            {
                tile.UpdateConnections(AreaComponent, new Vec2i(x, y));
                tile.Invalidated = false;
            }
                
            if (tile.InvalidateAdjacencies)
            {
                AreaComponent.InvalidateNeighbors(new Vec2i(x, y));
                tile.InvalidateAdjacencies = false;
            }
        }

        protected override void RenderBackground()
        {
            Pixel.FillRect(RenderArea, BackgroundColor);
            Pixel.FillRect(GridRenderArea, AreaBackgroundColor);
        }


        protected override void RenderOverlays()
        {
            if (!ShouldRenderOverlays)
                return;

            Rectangle area = new Rectangle(HX, HY, SpaceWidth, SpaceHeight);

            Pixel.FrameRect(area, VFK.EditorTileHighlightBorder);
        }

        protected override void RenderSpace(int x, int y, Rectangle area)
        {
            if (AreaComponent == null)
                return;
                
            Tile tile = AreaComponent[x, y];
            tile.RenderWithin(area);
        }

        protected override void RenderContentFrames()
        {
            if (GraphicsConfiguration.Debug)
                Pixel.FrameRect(GridRenderArea, Color.Aqua);

            base.RenderContentFrames();
        }
    }
}