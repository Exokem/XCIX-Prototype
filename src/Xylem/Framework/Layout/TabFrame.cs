
using Microsoft.Xna.Framework;
using Xylem.Framework.Control;
using Xylem.Graphics;
using Xylem.Functional;
using Xylem.Reference;

namespace Xylem.Framework.Layout
{
    public class TabFrame : Frame
    {
        HorizontalSequenceFrame Holder;
        Frame TabContent;

        SwitchButton SelectedTabButton;
        public string SelectedTabId { get; protected set; }

        public bool SizeTabsAutomatically { get; set; } = true;
        public float TabProportion { set => Holder.Proportion = value; }

        public TabFrame(string defaultTabId, string defaultTabLabel, Frame defaultTabContentFrame)
        {
            Holder = new HorizontalSequenceFrame()
            {
                BackgroundColor = XFK.Tertiary
            };

            Resizer = RatioResizer.ExpandBoth;

            WithTab(defaultTabId, defaultTabLabel, defaultTabContentFrame);

            Add(Holder);
        }

        protected virtual SwitchButton AssembleTabButton(string label, Frame contentFrame)
        {
            return new SwitchButton(label)
            {
                ActivationAction = frame => TabSelected(label, frame, contentFrame),
                ContentInsets = new Insets(5),
                Border = new SimpleBorder(1, 1, 1, 0)
            };
        }

        public TabFrame WithTab(string id, string label, Frame contentFrame)
        {
            SwitchButton tabButton = AssembleTabButton(label, contentFrame);

            Holder.Add(tabButton);
            Add(contentFrame);

            if (TabContent == null)
                TabSelected(id, tabButton, contentFrame);

            return this;
        }

        protected void SetTabContent(Frame frame)
        {
            TabContent = frame;
            Resize();
        }

        protected virtual void TabSelected(string id, Frame tabFrame, Frame contentFrame)
        {
            if (tabFrame is SwitchButton tabButton)
            {
                if (SelectedTabButton != null)
                {
                    SelectedTabButton.Selected = false;
                }

                SelectedTabButton = tabButton;
                SelectedTabId = id;
                SelectedTabButton.Selected = true;
                SetTabContent(contentFrame);
            }

        }

        public override void Resize()
        {
            base.Resize();

            Holder.W = CW;
            if (SizeTabsAutomatically)
                Holder.Proportion = 0.10F / GraphicsConfiguration.Scale;
            Holder.Resize();            

            if (TabContent != null)
            {
                TabContent.W = CW;
                TabContent.H = CH - Holder.RH;
                TabContent.OY = Holder.RH;
            }
        }

        protected override void UpdateContentFrames()
        {
            Holder.Update();
            TabContent?.Update();
        }

        protected override void RenderContentFrames()
        {
            TabContent?.Render();
            Holder.Render();

            // if (TabContent != null && GraphicsConfiguration.Debug)
                // Pixel.FillRect(TabContent.RenderArea, Color.Coral);
        }
    }
}