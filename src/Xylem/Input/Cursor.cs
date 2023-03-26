
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Xylem.Functional;

namespace Xylem.Input
{
    public static class CursorStateManager
    {
        private static bool _stateChangedThisUpdate = false;
        private static List<MouseCursor> _requestsThisUpdate = new List<MouseCursor>();

        static CursorStateManager()
        {
            UpdateDispatcher.Register(Update);
        }

        public static void RequestCursor(MouseCursor cursor)
        {
            if (_stateChangedThisUpdate)
                return;

            _requestsThisUpdate.Add(cursor);
        }

        private static void Update()
        {
            _stateChangedThisUpdate = false;

            MouseCursor finalCursor = MouseCursor.Arrow;

            foreach (var cursor in _requestsThisUpdate)
            {
                if (cursor != MouseCursor.Arrow)
                {
                    finalCursor = cursor;
                    break;
                }
            }

            _requestsThisUpdate.Clear();

            Mouse.SetCursor(finalCursor);

        }
    }
}