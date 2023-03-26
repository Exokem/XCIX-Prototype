
using Microsoft.Xna.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework;
using Xylem.Functional;
using Xylem.Reflection;

namespace Xylem
{
    [ModuleSource("Xylem")]
    public static class XylemModule
    {
        internal static readonly Frame Container;

        public static Frame ContainedFrame { get; internal set; }

        private static readonly LoadingFrame XylemLoadingFrame;

        static XylemModule()
        {
            Container = new Frame()
            {
                Resizer = RatioResizer.ExpandBoth,
                Borderless = true
            };

            XylemLoadingFrame = new LoadingFrame("Xylem-Title")
            {
                BackgroundColor = Color.Black
            };

            SetContainedFrame(XylemLoadingFrame); // Use this for end result
            // SetContainedFrame(Vitreous.VitreousModule.AreaEditor);
            // SetContainedFrame(Vitreous.VitreousModule.SectorEditor);
        }

        public static void SetContainedFrame(Frame frame)
        {
            if (ContainedFrame == frame)
                return;

            if (ContainedFrame != null)
                UpdateDispatcher.EnqueueUpdate(() => Container.Remove(ContainedFrame));
            
            if (frame == null)
                return;

            UpdateDispatcher.EnqueueUpdate(() => 
            {
                ContainedFrame = frame;
                Container.Add(frame);
                frame.Visibility = Visibilities.Invisible;
                Container.Resize();
                UpdateDispatcher.EnqueueDelayedUpdate(() => frame.Visibility = Visibilities.Visible, 10);
            });
        }

        public static LoadingFrame BindMenu(Frame menu)
        {
            XylemLoadingFrame.OnEnded = () => SetContainedFrame(menu);
            return XylemLoadingFrame;
        }
    }
}