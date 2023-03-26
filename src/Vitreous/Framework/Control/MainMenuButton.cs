
using System;
using Microsoft.Xna.Framework;
using Xylem.Framework.Control;
using Xylem.Graphics;
using Xylem.Vectors;
using Xylem.Functional;
using Vitreous.Reference;

namespace Vitreous.Framework.Control
{
    public class MainMenuButton : AnimatedButton
    {
        // public virtual Color TopLight => Hovered ? MenuColors.R1 : MenuColors.W1;
        // public virtual Color TopMed => Hovered ? MenuColors.R2 : MenuColors.W2;
        // public virtual Color TopDark => Hovered ? MenuColors.R3 : MenuColors.W3;
        public virtual Color TopLight => MenuColors.R1;
        public virtual Color TopMed => MenuColors.R2;
        public virtual Color TopDark => MenuColors.R3;

        // public virtual Color SideMed => Hovered ? MenuColors.R4 : MenuColors.W4;
        // public virtual Color SideDark => Hovered ? MenuColors.R5 : MenuColors.W5;
        public virtual Color SideMed => MenuColors.R4;
        public virtual Color SideDark => MenuColors.R5;

        public virtual Color BorderTopLight => Hovered ? MenuColors.Y1 : MenuColors.W1;
        public virtual Color BorderTopMed => Hovered ? MenuColors.Y2 : MenuColors.W2;
        public virtual Color BorderTopDark => Hovered ? MenuColors.Y3 : MenuColors.W3;

        public virtual Color BorderSideMed => Hovered ? MenuColors.Y4 : MenuColors.W4;
        public virtual Color BorderSideDark => Hovered ? MenuColors.Y5 : MenuColors.W5;

        // private int _pixelScale;

        // private const float _max = 190F;
        // private int _top = 160;
        private const float _topRatio = 4F / 5F;
        private const int _frameAmount = 3;

        private int _topHeight;
        private int _sideHeight => RH - _topHeight;
        // private int _renderedSideHeight => (int) ((double) _sideHeight * (1D - _pressTimer.GetProgress()));

        protected int HighlightWidth => (int) (HoverTimerProgress * (double) (RW + 2 * PixelScale));
        // protected int O => _pixelScale;

        public override Color TextColor 
        { 
            get
            {
                if (Hovered)
                    return MenuColors.Y2;
                else return MenuColors.W1;
            }
            set => base.TextColor = value; 
        }

        public virtual Color TextShadowColor => MenuColors.R3;

        // public virtual bool RenderLeftOutline { get; set; } = false;

        public override int CH => _topHeight;
        // public override int CY => base.CY + (int) ((double) _sideHeight * _pressTimer.GetProgress());
        int _renderedSideHeight => (int) (Math.Round((double) _sideHeight * (1D - _pressTimer.GetProgress())));
        public override int CY => base.CY + (RH - (_topHeight)) - _renderedSideHeight;

        public virtual int PixelScale { get; set; } = 4;

        private Stopwatch _pressTimer;

        public MainMenuButton()
        {
            HoverTimer = new Stopwatch(limit: 0.3D);
            Borderless = true;

            _pressTimer = new Stopwatch(0.08D);

            ActivatedProperty.AddObserver((prev, next) =>
            {
                if (next && !prev)
                    _pressTimer.Start();
                if (prev && !next)
                    _pressTimer.Reverse();
            });
        }

        protected override void FitText() {}

        public override void Resize()
        {
            base.Resize();
            // _pixelScale = (int) Math.Max(GraphicsConfiguration.Scale, 1F) * 4;
        }

        protected override void OnDimensionChanged()
        {
            base.OnDimensionChanged();
            _topHeight = (int) ((float) RH * _topRatio);
        }

        protected override void PostUpdate()
        {
            base.PostUpdate();
            TextScale = (float) Math.Round(((_topHeight / Typeface.CharHeight()) * 0.45F));

            // _pixelScale = PixelScale;
        }

        protected override void RenderBackground()
        {
            // Rectangle borderArea = RenderArea.Grow(PixelScale);
            int O = PixelScale;

            Rectangle outline = ContentArea.Grow(bottom: _renderedSideHeight + O, top: O, right: O, left: O);
            // if (RenderLeftOutline)
            //     outline = outline.Grow(left: O);
            Pixel.FillRect(outline, PrimaryColor);

            Rectangle sideArea = new Rectangle(CX, CY, CW, CH + _renderedSideHeight);
            Rectangle borderArea = sideArea.Grow(O);
            
            if (O < HighlightWidth)
            {
                borderArea.Width = HighlightWidth;

                Pixel.FillRect(borderArea, BorderTopMed);
                Pixel.FillRect(borderArea.Shrink(right: O, bottom: O), BorderTopLight);
                Pixel.FillRect(borderArea.Shrink(left: O, top: O), BorderTopDark);
                // Pixel.FrameRect(borderArea, PrimaryColor);
            }

            Pixel.FillRect(sideArea, SideMed);
            Pixel.FillRect(sideArea.Shrink(right: O, bottom: O), TopDark);
            Pixel.FillRect(sideArea.Shrink(left: O, top: O), SideDark);
            Pixel.FillRect(sideArea.Shrink(left: O, right: O, bottom: O), SideMed);

            Pixel.FillRect(ContentArea, TopMed);
            Pixel.FillRect(ContentArea.Shrink(right: O, bottom: O), TopLight);
            Pixel.FillRect(ContentArea.Shrink(left: O, top: O), TopDark);
            Pixel.FillRect(ContentArea.Shrink(O), TopMed);
        }

        protected override void RenderExtraContent()
        {
            if (LabelText != null)
            {
                Rectangle shadowArea = ContentArea;
                shadowArea.Offset(0, PixelScale);
                Text.RenderTextWithin(Typeface, LabelText, shadowArea, TextShadowColor, PixelScale, centerHorizontal: CenterText);
                Text.RenderTextWithin(Typeface, LabelText, ContentArea, TextColor, PixelScale, centerHorizontal: CenterText);
            }
        }

        protected override void RenderOverlays()
        {
            if (GraphicsConfiguration.Debug)
            {
                
            }
        }
    }
}