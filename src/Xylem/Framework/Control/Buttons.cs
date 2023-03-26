
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Data;
using Xylem.Graphics;
using Xylem.Functional;
using Xylem.Reference;
using Xylem.Framework.Layout;

namespace Xylem.Framework.Control
{
    public class Button : Label
    {
        protected override MouseCursor HoveredCursor => MouseCursor.Hand;

        public Button(string label = null) : base(label)
        {
        }

        public virtual Color ButtonBackgroundActivated { get; set; } = XFK.ButtonBackgroundActivated;

        public virtual Color ButtonBackgroundHovered { get; set; } = XFK.ButtonBackgroundHovered;
        
        public virtual Color ButtonBackgroundSelected { get; set; } = XFK.SwitchButtonBackgroundSelected;

        public virtual Color ButtonBackground { get; set; } = XFK.ButtonBackground;

        public override Color BackgroundColor 
        { 
            get 
            {
                if (Activated)
                    return ButtonBackgroundActivated;
                else if (Hovered)
                    return ButtonBackgroundHovered;
                else 
                    return base.BackgroundColor == Color.Transparent ? ButtonBackground : base.BackgroundColor;
            }
            set => base.BackgroundColor = value; 
        }

        public virtual Color ButtonTextActivated { get; set; } = XFK.ButtonTextActivated;
        public virtual Color ButtonTextHovered { get; set; } = XFK.ButtonTextHovered;
        public virtual Color ButtonTextSelected { get; set; } = XFK.SwitchButtonTextSelected;
        public virtual Color ButtonText { get; set; } = XFK.ButtonText;

        public override Color TextColor 
        { 
            get 
            {
                if (Activated)
                    return ButtonTextActivated;
                else if (Hovered || Focused)
                    return ButtonTextHovered;
                else 
                    return ButtonText;
            }
            set => base.TextColor = value; 
        }
    }

    public class SwitchButtonItem : IListItem<SwitchButton>
    {
        public bool Selected { get => Item.Selected; set => Item.Selected = value; }
        
        public ListFrame<SwitchButton> ItemContainer { get; set; }

        public SwitchButton Item { get; private set; }

        public Frame Frame => Item;

        public SwitchButtonItem(SwitchButton item)
        {
            Item = item;
            Item.ActivationAction = frame => ItemContainer.Select(this);
            Item.Border.Top = 0;
        }

        public void OnSelected()
        {
            Item.OnSelected?.Invoke(Item);
        }
    }

    public class SwitchButton : Button
    {
        public bool Selected { get; set; }

        public Receiver<SwitchButton> OnSelected { get; set; }

        public override Color BackgroundColor 
        { 
            get 
            {
                if (Selected)
                    // return Hovered ? Options.UIAccent * 0.6F : Options.UIAccent;
                    return ButtonBackgroundSelected;
                else 
                    return base.BackgroundColor;
            }
            set => base.BackgroundColor = value; 
        }

        public override Color TextColor 
        { 
            get 
            {
                if (Selected)
                    return ButtonTextSelected;
                else 
                    return base.TextColor;
            }
            set => base.TextColor = value; 
        }

        public SwitchButton(string label = "") : base(label)
        {
            ActivationAction = frame => Selected = !Selected;
        }

        // protected override void OnActivated()
        // {
        //     Selected = !Selected;
        // }
    }

    // public class CheckBox : Frame
    // {
    //     public bool Selected { get; set; }

    //     // public override int MX => MW < base.MW ? (base.MX + (base.MW / 2) - (MW / 2)) : base.MX;
    //     // public override int MY => MH < base.MH ? (base.MY + (base.MH / 2) - (MH / 2)) : base.MY;

    //     // public override int MW => Math.Min(base.MW, base.MH);
    //     // public override int MH => Math.Min(base.MW, base.MH);

    //     // public override int CX => CW < base.CW ? (base.CX + (base.CW / 2) - (CW / 2)) : base.CX;
    //     // public override int CY => CH < base.CH ? (base.CY + (base.CH / 2) - (CH / 2)) : base.CY;

    //     // public override int CW => Math.Min(base.CW, base.CH);
    //     // public override int CH => Math.Min(base.CW, base.CH);

    //     public Receiver<bool> OnSelected = v => {};

    //     public CheckBox()
    //     {
    //         ContentInsets = new Insets(5);
    //         ContentMargin = new Insets(3);
    //     }

    //     public override Color BackgroundColor 
    //     { 
    //         get 
    //         {
    //             if (Hovered)
    //                 return Options.UIBackgroundHover;
    //             else 
    //                 return base.BackgroundColor == Color.Transparent ? Options.UIBright : base.BackgroundColor;
    //         }
    //         set => base.BackgroundColor = value; 
    //     }

    //     protected override void OnActivated()
    //     {
    //         Selected = !Selected;
    //         OnSelected(Selected);
    //     }

    //     protected override void RenderBackground()
    //     {
    //         Pixel.FillRect(MarginArea, BackgroundColor);
    //     }

    //     protected override void RenderContentFrames()
    //     {
    //         if (Selected)
    //             Pixel.FillRect(MarginArea.CenteredWithin(0.75F, 0.75F), Hovered ? Options.UIAccent : Options.UIPrimary);
    //     }
    // }

    // public class CheckButton : Frame
    // {
    //     protected readonly CheckBox CheckBox;
    //     protected readonly Label Label;

    //     public Receiver<bool> OnSelected 
    //     { 
    //         set
    //         {
    //             if (value != null)
    //                 CheckBox.OnSelected = value;
    //         }
    //     }

    //     public CheckButton(string label, float textScale = 1.0F)
    //     {
    //         CheckBox = new CheckBox()
    //         {
    //             Borderless = true
    //         };
    //         Label = new Label(label)
    //         {
    //             CenterText = false, ContentInsets = new Insets(5),
    //             Borderless = true,
    //             TextScale = textScale
    //         };

    //         ContentInsets = new Insets(0, 5, 0, 5);

    //         Add(CheckBox);
    //         Add(Label);
    //     }

    //     public override int MeasuredWidth => base.MeasuredWidth + Label.MeasuredWidth + Label.Typeface.ScaledHeight(Label.TextScale * 0.75F);

    //     public override int MeasuredHeight => base.MeasuredHeight + Label.MeasuredHeight;

    //     protected override void OnResized()
    //     {
    //         CheckBox.W = CheckBox.H = Label.Typeface.ScaledHeight(Label.TextScale * 0.75F);
    //         CheckBox.OY = (CH - CheckBox.H) / 2;
    //         Label.OX = CheckBox.RW;
    //         Label.W = CW - CheckBox.RW;
    //         Label.H = CH;
    //     }

    //     protected override void RenderContentFrames()
    //     {
    //         CheckBox.Render();
    //         Label.Render();
    //     }
    // }
}