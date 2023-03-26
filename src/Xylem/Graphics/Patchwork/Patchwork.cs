
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Xylem.Component;
using Xylem.Vectors;
using Xylem.Functional;
using Xylem.Input;
using Xylem.Reflection;

namespace Xylem.Graphics.Patchwork
{
    /**
     * Patchwork connectors are responsible for managing the 'patches' of a texture
     * resource, particularly for use in visually connecting adjacent components.
     * 
     * Patches are herein defined as portions of a texture resource that are overlaid
     * on top of a base texture when a connection exists in a particular direction. The
     * base texture is not known to patchwork connectors.
     *
     * The primary intent of the existence of this class is to enable more flexbility with
     * how the textures of components that connect visually are processed and organized, 
     * placing the logic for creating patchworks into a sort of black box as far as 
     * actual rendering logic need be concerned.
     */
    public abstract class AbstractPatchworkConnector
    {
        // Associates Directions with specific patches of the texture resource.
        private readonly Dictionary<Direction, Rectangle> _resourcePatches;

        private bool _finalized;

        protected readonly TextureResource Resource;

        protected AbstractPatchworkConnector(TextureResource resource)
        {
            // Order matters, leave as-is
            _resourcePatches = new Dictionary<Direction, Rectangle>();
            Resource = resource;
        }

        public static AbstractPatchworkConnector Create(string subclassType, params object[] data)
        {
            AbstractPatchworkConnector patchwork = Introspector.Instantiate<AbstractPatchworkConnector>(subclassType, typeof(AbstractPatchworkConnector), data);

            patchwork.PatchConnections();
            patchwork._finalized = true;

            return patchwork;
        }

        public static AbstractPatchworkConnector Create(Type subclassType, params object[] data)
        {
            AbstractPatchworkConnector patchwork = Introspector.Instantiate<AbstractPatchworkConnector>(subclassType, typeof(AbstractPatchworkConnector), data);

            patchwork.PatchConnections();
            patchwork._finalized = true;

            return patchwork;
        }

        /**
         * Divides the texture resource into patches, assigning each to a specific 
         * connection direction. 
         */
        protected abstract void PatchConnections();

        /**
         * Provides the patch within the provided area that corresponds to the overlay
         * for connections in the specified direction. 
         */
        public abstract Rectangle GetPatchWithin(Rectangle area, Direction direction);

        public Rectangle GetResourcePatch(Direction direction) => _resourcePatches[direction];

        public void RenderWithin(Rectangle area, Direction direction)
        {
            if (!_resourcePatches.ContainsKey(direction))
                return;

            Rectangle source = GetResourcePatch(direction);
            Rectangle dest = GetPatchWithin(area, direction);

            Resource.IRenderWithinPartial(dest, source);
        }

        public void RenderWithin(Rectangle area, IEnumerable<Direction> directions)
        {
            foreach (Direction dir in directions)
                RenderWithin(area, dir);
        }

        public void RenderWithinDynamic(Rectangle area, Rectangle dynamicSource, Direction direction)
        {
            Rectangle sourcePatch = GetPatchWithin(dynamicSource, direction);
            Rectangle destPatch = GetPatchWithin(area, direction);

            Resource.IRenderWithinPartial(destPatch, sourcePatch);
        }

        public void RenderDebugOverlay(Rectangle area, IEnumerable<Direction> directions)
        {
            if (GraphicsConfiguration.Debug)
            {
                foreach (var direction in directions)
                {
                    Color color = new Color(0, 0, 0, 50);
                    if (direction.IsUnion)
                        color.R = 255;
                    else if (direction.IsIntersection)
                        color.G = 255;
                    else 
                        color.B = 255;
                    if (InputProcessor.CursorWithinExclusive(area))
                        Pixel.FillRect(GetPatchWithin(area, direction), color);
                }
            }
        }

        /**
         * Provides internal access to set the resource patch for a specific direction. 
         * This accessor will be locked after the PatchConnections function is called for
         * a new patchwork connector.
         */
        protected void SetResourcePatch(Direction direction, Rectangle area) 
        {
            if (!_finalized)
                _resourcePatches[direction] = area;
        }
    }

    public abstract class SkewedPatchworkConnector : AbstractPatchworkConnector
    {
        protected int Numerator;
        protected int Denominator;

        protected SkewedPatchworkConnector(TextureResource resource, int numerator = 7, int denominator = 24) : base(resource)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public int ApplyHeightSkew(int height, Direction direction) => ApplyHeightSkew(height, Numerator, Denominator, direction);

        // Summary:
        //      Determines the height of a top or bottom section when the horizontal axis
        //      of the patchwork texture is skewed from the center. This will be the case
        //      usually for top-down textures that are not viewed from above directly, but
        //      rather with a consistent proportion between the top and bottom 'halves'.
        //      By default, the top 'half' is 7 of 24 pixels and the bottom 'half' is the
        //      remaining 17 of 24 pixels. 
        public static int ApplyHeightSkew(int height, int numerator, int denominator, Direction direction)
        {
            if (direction.IsUp)
                return numerator * height / denominator;
            else if (direction.IsDown)
                return (denominator - numerator) * height / denominator;
            return height;
        }
    }

    /**
     * The Overhead Patchwork Connector is specialized to create connection patchworks
     * for ideal overhead texture resources. This means that the corner connections (for
     * union and intersection directions) will all be the same size.
     *
     * The format for texture resources accepted by this patchwork connector is as follows.
     * 1. The texture is conventially divided into four, horizontally adjacent, sections
     * 2. The first section contains connections for each of the four union directions. 
     *    These are inverted corners in other words. 
     * 3. The second section contains connections for the vertical directions (up, down).
     *    This section will be split into two horizontal halves.
     * 4. The third section contains connections for the horizontal directions (left, right).
     *    This section will be split into two vertical halves.
     * 5. The fourth and final section contains connections for the four intersection
     *    directions. These cover cases when a texture has two adjacent orthogonal 
     *    connections as well as a diagonal connection between those two. (e.g. left, up, 
     *    and up-left)
     */
    public class OverheadPatchworkConnector : SkewedPatchworkConnector
    {
        public OverheadPatchworkConnector(TextureResource resource) : base(resource) {}

        public OverheadPatchworkConnector(TextureResource resource, int numerator, int denominator) : base(resource, numerator, denominator) {}

        protected override void PatchConnections()
        {
            int secWidth = Resource.Texture.Width / 4;
 
            Rectangle unionSection     = new Rectangle(0 * secWidth, 0, secWidth, secWidth);
            Rectangle upDownSection    = new Rectangle(1 * secWidth, 0, secWidth, secWidth);
            Rectangle leftRightSection = new Rectangle(2 * secWidth, 0, secWidth, secWidth);
            Rectangle intersectSection = new Rectangle(3 * secWidth, 0, secWidth, secWidth);

            foreach (Direction union in Direction.Unions())
                SetResourcePatch(union, GetPatchWithin(unionSection, union));

            foreach (Direction cardinal in Direction.Cardinals())
            {
                if (cardinal.IsVertical)
                    SetResourcePatch(cardinal, GetPatchWithin(upDownSection, cardinal));
                else if (cardinal.IsHorizontal)
                    SetResourcePatch(cardinal, GetPatchWithin(leftRightSection, cardinal));
            }

            foreach (Direction intersection in Direction.Intersections())
                SetResourcePatch(intersection, GetPatchWithin(intersectSection, intersection));
        }

        public override Rectangle GetPatchWithin(Rectangle area, Direction direction)
        {
            int x = area.X, y = area.Y, w = area.Width, h = area.Height;

            int width = w;

            if (direction.IsUnion || direction.IsIntersection || direction.IsHorizontal)
                width = w / 2;

            int height = h;

            if (direction.IsUnion || direction.IsIntersection || direction.IsVertical)
                height = ApplyHeightSkew(h, direction);

            int sx = x + System.Math.Max(direction.OX, 0) * (w / 2);
            int sy = y + System.Math.Max(direction.OY, 0) * (Numerator * h / Denominator);

            return new Rectangle(sx, sy, width, height);
        }
    }

    /**
     * This Generic Patchwork Connector devotes an entire section of the texture resource
     * to each connection patch. The order is the same as the order of Directions in the 
     * set of all Direction values.
     */
    public sealed class GenericPatchworkConnector : AbstractPatchworkConnector
    {
        public GenericPatchworkConnector(TextureResource resource) : base(resource) {}

        protected override void PatchConnections()
        {
            int width = Resource.Texture.Width / Direction.Count;

            Rectangle section = new Rectangle(0, 0, width, width);

            foreach (Direction direction in Direction.Values())
            {
                SetResourcePatch(direction, section);
                section.Offset(width, 0);
            }
        }

        public override Rectangle GetPatchWithin(Rectangle area, Direction direction) => area;
    }
}