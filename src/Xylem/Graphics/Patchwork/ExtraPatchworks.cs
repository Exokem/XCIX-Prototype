
using Microsoft.Xna.Framework;
using Xylem.Component;
using Xylem.Vectors;

namespace Xylem.Graphics.Patchwork
{
    // Summary:
    //      This Patchwork Connector manages connections along the main axes. Only 
    //      cardinal directions are considered by this connector.
    //
    //      The default format of resources used by this connector consists of two 
    //      horizontally adjacent sections of equal width and height. The first
    //      defines the LEFT/RIGHT overlays, and the second defines the UP/DOWN
    //      overlays. 
    public class AxisOverheadPatchworkConnector : SkewedPatchworkConnector
    {
        public AxisOverheadPatchworkConnector(TextureResource resource) : base(resource) {}

        protected override void PatchConnections()
        {
            int sectionWidth = Resource.Texture.Width / 2;
            int sectionHeight = Resource.Texture.Height;

            Rectangle leftRightSection = new Rectangle(0 * sectionWidth, 0, sectionWidth, sectionHeight);

            Rectangle upDownSection = new Rectangle(1 * sectionWidth, 0, sectionWidth, sectionHeight);

            foreach (Direction cardinal in Direction.Cardinals())
            {
                Rectangle area = cardinal.IsVertical ? upDownSection : leftRightSection;

                SetResourcePatch(cardinal, GetPatchWithin(area, cardinal));
            }
        }

        public override Rectangle GetPatchWithin(Rectangle area, Direction direction)
        {
            int x = area.X;
            int y = area.Y;
            int w = area.Width;
            int h = area.Height;

            if (!direction.IsCardinal)
                return new Rectangle();

            if (direction.IsHorizontal)
            {
                w /= 2;

                if (direction == Direction.RIGHT)
                    x += w;
            }

            else if (direction.IsVertical)
            {
                h = ApplyHeightSkew(h, direction);

                if (direction == Direction.DOWN)
                    y += area.Height - h;
            }

            return new Rectangle(x, y, w, h);
        }
    }
}