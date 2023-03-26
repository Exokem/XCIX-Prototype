
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Xylem.Functional;
using Xylem.Vectors;
using Xylem.Input;
using Xylem.Framework;

namespace Vitreous.Framework.Editor
{
    public interface IToolFrame<V>
    {
        IEditorTool<V> ActiveTool { get; set; }
        V Component { get; set; }
        Vec2i TargetPosition { get; }

        Stack<ReversibleAction> UndoStack { get; }
        Stack<ReversibleAction> RedoStack { get; }

        void UpdateActiveTool()
        {   
            if (Component == null || ActiveTool == null || !ActiveTool.Applicable(Component))
                return;

            if (ActiveTool.Edit(Component, TargetPosition))
            {
                EnqueueToolAction();
                ActiveTool = ActiveTool.End();
            }
        }

        void UpdateFocusedToolInputs()
        {
            if (I.CONTROL_Z.RequestClaim())
                Undo();

            if (I.CONTROL_Y.RequestClaim())
                Redo();
        }

        bool RenderToolOverlays(Rectangle area)
        {
            if (ActiveTool == null)
                return false;

            return ActiveTool.RenderHoverData(area);
        }

        void EnqueueToolAction()
        {
            // These two variables are required to resolve a nebulous issue related to
            // reference typing
            // Without these variables, the tool and space used to undo/redo will be those
            // at the time of undo/redo, not at the time of the action enqueue
            IEditorTool<V> tool = ActiveTool;
            Vec2i hovered = TargetPosition;

            UndoStack.Push(new ReversibleAction
            (
                () => tool.Edit(Component, hovered), 
                () => tool.Restore(Component, hovered),
                tool.ToString()
            ));

            RedoStack.Clear();
        }

        void Undo()
        {
            ReversibleAction action;
            if (UndoStack.TryPop(out action))
            {
                action.Revert();
                RedoStack.Push(action);
                NotificationManager.EnqueueNotification($"Undo {action.ToString()}");
            }
        }

        void Redo()
        {
            ReversibleAction action;
            if(RedoStack.TryPop(out action))
            {
                action.Invoke();
                UndoStack.Push(action);
                NotificationManager.EnqueueNotification($"Redo {action.ToString()}");
            }
        }
    }
}