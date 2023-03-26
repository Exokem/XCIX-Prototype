
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Graphics;
using Xylem.Data;
using Xylem.Component;
using Xylem.Functional;
using Xylem.Reference;
using Xylem.Registration;
using Xylem.Framework.Layout;

namespace Xylem.Framework.Control
{
    public class TextureResourceFrame : Frame
    {
        public TextureResource Resource { get; set; }

        public float ResourceScale 
        {
            set
            {
                ScaleX = ScaleY = value;
            }
        }

        public float ScaleX { get; set; } = 1.0F;
        public float ScaleY { get; set; } = 1.0F;

        public bool AutoScale { get; set; } = false;
        public bool UseLargerDimension { get; set; } = false;

        public virtual Color OverlayColor { get; set; } = Color.White;

        public int IW => (int) (Resource == null ? 1 : Resource.Texture.Width);
        public int IH => (int) (Resource == null ? 1 : Resource.Texture.Height);

        public float IR => (float) IW / (float) IH;

        public override int MeasuredWidth => base.MeasuredWidth + SW;
        public override int MeasuredHeight => base.MeasuredHeight + SH;

        // Scaled values
        protected virtual int SX => CX + (CW - SW) / 2;
        protected virtual int SY => CY + (CH - SH) / 2;
        protected virtual int SW => (int) ((float) IW * ScaleX);
        protected virtual int SH => (int) ((float) IH * ScaleY);

        public TextureResourceFrame(TextureResource resource = null)
        {
            Resource = resource;
        }

        public TextureResourceFrame(string resourceIdentifier = null)
        {
            if (resourceIdentifier != null)   
                Resource = R.Textures[resourceIdentifier];
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            if (AutoScale)
            {
                float areaW = System.Math.Max(IW, CW);
                float areaH = System.Math.Max(IH, CH);

                float scaleW = areaW / (float) IW;
                float scaleH = areaH / (float) IH;

                ResourceScale = System.Math.Min(scaleW, scaleH);

                // float spaceRatio = scaleW / scaleH;

                // if (IR < spaceRatio)
                // {
                //     // Too wide
                    
                //     if (UseLargerDimension)
                //     {
                //         ScaleX = scaleW;
                //         ScaleY = scaleW / IR;
                //     }

                //     else 
                //     {
                //         ScaleX = scaleH * IR;
                //         ScaleY = scaleH;
                //     }
                // }

                // else if (spaceRatio < IR)
                // {
                //     // Too narrow

                //     if (UseLargerDimension)
                //     {
                //         ScaleX = scaleH * IR;
                //         ScaleY = scaleH;
                //     }

                //     else 
                //     {
                //         ScaleX = scaleW;
                //         ScaleY = scaleW / IR;
                //     }
                // }

                // else
                // {
                //     if (UseLargerDimension)
                //     {
                //         ScaleX = ScaleY = Math.Max(scaleW, scaleH);
                //     }

                //     else 
                //     {
                //         ScaleX = ScaleY = Math.Min(scaleW, scaleH);
                //     }
                // }
            }
        }

        protected override void RenderExtraContent()
        {
            if (Resource == null)
                return;
            
            GraphicsContext.Render(Resource.Texture, new Rectangle(SX, SY, SW, SH), OverlayColor);
        }
    }

    public class IconFrame : TextureResourceFrame
    {
        public override Color OverlayColor { get; set; } = XFK.IconColor;

        public IconFrame(TextureResource resource = null) : base(resource) {}

        public IconFrame(string resourceIdentifier = null) : base(resourceIdentifier) {}
    }

    public class IconButton : IconFrame, ISelected
    {
        protected override MouseCursor HoveredCursor => MouseCursor.Hand;

        public bool Selected { get; set; } = false;
        public bool Selectable { get; set; } = false;

        public Update OnSelected { get; set; } = () => {};

        public override Color BackgroundColor 
        { 
            get 
            {
                if (Activated || (Selectable && Selected))
                    return XFK.IconButtonBackgroundActivated;
                else if (Hovered)
                    return XFK.IconButtonBackgroundHovered;
                else 
                    return base.BackgroundColor == Color.Transparent ? XFK.IconButtonBackground : base.BackgroundColor;
            }
            set => base.BackgroundColor = value; 
        }

        public override Color OverlayColor 
        { 
            get 
            {
                if (Activated || (Selectable && Selected))
                    return XFK.IconButtonIconActivated;
                else if (Hovered || Focused)
                    return XFK.IconButtonIconHovered;
                else 
                    return XFK.IconColor;
            }
            set => base.OverlayColor = value; 
        }

        public IconButton(TextureResource resource = null) : base(resource)
        {
            ActivationAction = frame => Selected = !Selected;
        }
    }

    public class IconItem : IListItem<TextureResource>
    {
        // public override int MeasuredWidth => Item.MeasuredWidth;
        // public override int MeasuredHeight => Item.MeasuredHeight;

        public bool Selected { get => Frame.Focused; set => Frame.Focused = value; }

        public ListFrame<TextureResource> ItemContainer { get; set; }

        public TextureResource Item => Resource;

        public Frame Frame { get; set; }

        protected readonly TextureResource Resource;
        // protected readonly IconFrame 
        
        public IconItem(TextureResource resource, float iconScale)
        {
            Resource = resource;
            Frame = new IconFrame(resource)
            {
                ResourceScale = iconScale
            };
        }

        public void OnSelected() {}
    }

    public class IconButtonItem : IListItem<IconButton>
    {
        public bool Selected { get => Item.Selected; set => Item.Selected = value; }

        public ListFrame<IconButton> ItemContainer { get; set; }

        public IconButton Item { get; }

        public Frame Frame => Item;

        public IconButtonItem(IconButton iconButton)
        {
            Item = iconButton;
            Item.Selectable = true;
            Item.ActivationAction = frame =>
            {
                ItemContainer.Select(this);
            };
        }

        public void OnSelected() 
        {
            Item.OnSelected();
        }
    }
}