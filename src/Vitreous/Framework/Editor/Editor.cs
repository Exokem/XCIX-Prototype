
using Xylem.Framework;
using Xylem.Framework.Layout;
using Xylem.Framework.Control;
using Xylem.Input;
using Vitreous.Reference;

namespace Vitreous.Framework.Editor
{
    public partial class Editor : Frame
    {
        TabFrame TabHolder;

        internal string CurrentEditor => TabHolder.SelectedTabId;

        public Editor()
        {
            Borderless = true;
            Resizer = RatioResizer.ExpandBoth;

            AssembleSectorEditor();
            AssembleAreaEditor();

            TabHolder = new TabFrame(VK.Sector, "Sector Editor", SectorEditorHolder)
                .WithTab(VK.Area, "Area Editor", AreaEditorHolder);

            AddAll(TabHolder);
        }

        protected override void UpdateGlobalInputs()
        {
            if (I.CONTROL_S.RequestClaim())
            {
                EditorData.Save();
            }
        }

        public override void Resize()
        {
            base.Resize();

            ResizeAreaEditor();
        }
    }
}