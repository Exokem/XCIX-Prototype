
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Graphics;
using Xylem.Input;
using Vitreous.Component.Spatial;
using Vitreous.Reference;

namespace Vitreous.Framework.Game
{
    public class SectorFrame : GridMotiveFrame
    {
        public Sector SectorComponent;

        public override int GridWidth => Sector.GridWidth;
        public override int GridHeight => Sector.GridHeight;

        public override int BaseSpaceWidth => SpatialOptions.SectorAreaWidth;
        public override int BaseSpaceHeight => SpatialOptions.SectorAreaHeight;

        public override int SpaceWidth { get; set; } = SpatialOptions.SectorAreaWidth;
        public override int SpaceHeight { get; set; } = SpatialOptions.SectorAreaHeight;

        protected override void UpdateSpace(int x, int y)
        {
        }

        protected override void RenderSpace(int x, int y, Rectangle area)
        {
            if (SectorComponent != null)
            {
                Area areaComponent = SectorComponent[x, y];

                Color color = areaComponent == null ? VFK.SectorEditorEmptyArea : VFK.SectorEditorArea;

                Pixel.FillRect(area, color);
            }
        }

        protected override void RenderBackground()
        {
            Pixel.FillRect(RenderArea, VFK.EditorViewportBackground);
            Pixel.FillRect(GridRenderArea, VFK.EditorAreaBackground);
        }
    }
}