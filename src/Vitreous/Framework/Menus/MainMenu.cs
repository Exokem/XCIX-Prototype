
using Microsoft.Xna.Framework;
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Registration;
using Xylem.Graphics;
using Vitreous.Framework.Control;
using Vitreous.Reference;

namespace Vitreous.Framework.Menus
{
    internal sealed partial class MainMenuFrame : Frame
    {
        HorizontalSplitFrame ButtonHolder;

        TextureResourceFrame Title;

        MainMenuButton Opt, Start, Load, Edit, Exit;

        internal MainMenuFrame()
        {
            BackgroundColor = MenuColors.B5;

            Resizer = RatioResizer.ExpandBoth;

            Title = new TextureResourceFrame(resource: null)
            {
                HorizontalAlignment = new CenteredHorizontalAlignment(),
                VerticalAlignment = new CenteredVerticalAlignment(),
                Resizer = RatioResizer.ExpandBoth,
                Borderless = true
            };

            ButtonHolder = new HorizontalSplitFrame()
            {
                Resizer = new WidthRatio(secondary: new HeightRatio(0.07F)),
                HorizontalAlignment = new CenteredHorizontalAlignment(),
                VerticalAlignment = new BottomAlignment(),
                OY = 1,
                Borderless = true
            };

            Opt = new MainMenuButton()
            {
                LabelText = "",
                Typeface = R.Typefaces["sushimono"],
                ExpandHorizontal = true
            };
            Start = new MainMenuButton()
            {
                LabelText = "",
                Typeface = R.Typefaces["sushimono"],
                ExpandHorizontal = true
            };
            Load = new MainMenuButton()
            {
                LabelText = "",
                Typeface = R.Typefaces["sushimono"],
                ExpandHorizontal = true
            };
            Edit = new MainMenuButton()
            {
                LabelText = "tilemap editor",
                Typeface = R.Typefaces["sushimono"],
                ExpandHorizontal = true,
                ActivationAction = frame => Xylem.XylemModule.SetContainedFrame(VitreousModule.Editor)
            };
            Exit = new MainMenuButton()
            {
                LabelText = "exit",
                Typeface = R.Typefaces["sushimono"],
                ExpandHorizontal = true,
                ActivationAction = Vitreous.VitreousModule.Exit
            };

            ButtonHolder.AddAll(Opt, Start, Load, Edit, Exit);

            AddAll(ButtonHolder, Title);
        }

        public override void Resize()
        {
            int scale = GraphicsConfiguration.PixelScale;

            Opt.TextScale = Start.TextScale = Load.TextScale = Edit.TextScale = Exit.TextScale = scale;

            Opt.PixelScale = Start.PixelScale = Load.PixelScale = Edit.PixelScale = Exit.PixelScale = scale;

            ButtonHolder.Spacing = scale * 2;
            ButtonHolder.VerticalAlignment.Offset = scale + 1;
            ButtonHolder.ContentInsets = new Insets(left: scale + 1, right: scale + 1);

            Title.ResourceScale = scale;

            base.Resize();

        }
    }
}