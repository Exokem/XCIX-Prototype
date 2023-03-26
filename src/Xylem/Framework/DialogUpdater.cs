
using System;
using System.Collections.Generic;
using Xylem.Framework.Control;
using Xylem.Functional;

namespace Xylem.Framework
{
    public static class BlockingUpdater
    {
        private static readonly Stack<DialogFrame> _dialogs;
        private static ContextMenu _activeMenu;

        static BlockingUpdater()
        {
            _dialogs = new Stack<DialogFrame>();
        }

        public static void PostDialog(DialogFrame dialog)
        {
            _dialogs.Push(dialog);
            UpdateDispatcher.EnqueueUpdate(dialog.Resize);
        }

        [Obsolete]
        public static void SetContextMenu(ContextMenu menu) => _activeMenu = menu;
        [Obsolete]
        public static void UnsetContextMenu(ContextMenu menu)
        {
            if (_activeMenu == menu)
                _activeMenu = null;
        }
        

        public static void CloseDialog(bool shouldClose, DialogFrame dialog)
        {
            if (shouldClose)
                _dialogs.Pop();
        }

        // Returns true when subsequent updates should not be blocked
        public static bool Update()
        {
            if (_activeMenu != null)
            {
                _activeMenu.Update();
                return false;
            }

            if (_dialogs.Count != 0)
            {
                _dialogs.Peek().Update();
                return false;
            }

            return true;
        }

        public static void Render()
        {
            foreach (DialogFrame dialog in _dialogs)
            {
                dialog.Render();
            }

            _activeMenu?.Render();
        }
    }
}