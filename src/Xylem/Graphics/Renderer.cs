
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Xylem.Graphics
{
    public interface IRenderedComponent
    {
        abstract Texture2D Texture { get; }
    }

    public static class RenderedComponentExtensions
    {
        public static void IRenderWithin<V>(this V v, Rectangle area) where V : IRenderedComponent
        {
            GraphicsContext.Render(v.Texture, area);
        }

        public static void IRenderWithinPartial<V>(this V v, Rectangle dest, Rectangle source) where V : IRenderedComponent
        {
            GraphicsContext.RenderPartial(v.Texture, dest, source);
        }
    }
}