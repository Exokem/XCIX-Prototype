
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Framework;
using Xylem.Graphics;
using Xylem.Component;
using Xylem.Vectors;
using Xylem.Framework.Layout;
using Xylem.Registration;
using Xylem.Functional;
using Vitreous.Component.Composite;
using Vitreous.Reference;

namespace Vitreous.Framework.Layout
{
    // TODO: merge these classes

    public class VisibleEntryItem<V> : ListFrameItem<V> where V : RegistryEntry
    {
        private readonly Function<V, TextureResource> _textureProvider;

        private int _iconWidth, _textOffset;

        public VisibleEntryItem(V item, Function<V, TextureResource> textureProvider) : base(item)
        {
            ContentInsets = new Insets(5);
            RenderDefaultContent = false;
            _textureProvider = textureProvider;
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();

            _iconWidth = Math.Min(CW, CH);
            _iconWidth -= _iconWidth % SpatialOptions.TileWidth;
            _textOffset = ContentInsets.Width + _iconWidth;
        }

        protected override void RenderContentFrames()
        {
            base.RenderContentFrames();

            TextureResource texture = _textureProvider?.Invoke(Item);

            if (texture != null)
                texture.IRenderWithin(new Rectangle(CX, CY, _iconWidth, _iconWidth));

            // _textureProvider?.Invoke(Item).IRenderWithin();

            Rectangle textArea = new Rectangle(CX + _textOffset, CY, CW - _textOffset, CH);

            Text.RenderTextWithin(Typeface, Item.Identifier, textArea, TextColor, centerHorizontal: false);
        }
    }
}