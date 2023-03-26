
using Xylem.Framework;
using Xylem.Reflection;
using Xylem;
using Vitreous.Framework.Menus;
using Vitreous.Framework.Editor;

namespace Vitreous
{
    [ModuleSource("Vitreous")]
    internal static class VitreousModule
    {
        private static Frame MainFrame;

        public static readonly MainMenuFrame MainMenuFrame;

        public static readonly AreaEditor AreaEditor;

        public static readonly OldSectorEditor SectorEditor;

        public static readonly Editor Editor;

        static VitreousModule()
        {
            MainMenuFrame = new MainMenuFrame();
            AreaEditor = new AreaEditor();
            SectorEditor = new OldSectorEditor();
            Editor = new Editor();

            XylemModule.BindMenu(MainMenuFrame);
            // XylemModule.SetContainedFrame(SectorEditor);
            // XylemModule.SetContainedFrame(AreaEditor);
            // XylemModule.SetContainedFrame(Editor);
        }

        public static void Exit(Frame sender)
        {
            Source.Instance.Exit();
        }
    }
}