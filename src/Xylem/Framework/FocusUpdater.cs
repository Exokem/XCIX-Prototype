
using static Xylem.Input.InputProcessor;

namespace Xylem.Framework
{
    public abstract class FocusUpdater
    {
        // Summary:
        //      Updates the focus of the provided frame.
        //      
        // Preconditions: 
        //      The frame is focusable. 
        public abstract void UpdateFocus(Frame frame);
    }

    public class StandardMouseLeft : FocusUpdater
    {
        public override void UpdateFocus(Frame frame)
        {
            if (!frame.CursorWithin && MouseLeft.Clicked)
                FocusManager.SetUnfocused(frame);
            else if (MouseLeft.Clicked)
                FocusManager.SetFocused(frame);
        }
    }

    public class GainFocusOnMouseLeft : FocusUpdater
    {
        public override void UpdateFocus(Frame frame)
        {
            if (!frame.CursorWithin)
                return;
            if (MouseLeft.Clicked)
                FocusManager.SetFocused(frame);
        }
    }

    public class GainFocusOnMouseMiddle : FocusUpdater
    {
        public override void UpdateFocus(Frame frame)
        {
            if (!frame.CursorWithin)
                return;
            if (MouseMiddle.Clicked)
                FocusManager.SetFocused(frame);
        }
    }

    public class GainFocusOnMouseRight : FocusUpdater
    {
        public override void UpdateFocus(Frame frame)
        {
            if (!frame.CursorWithin)
                return;
            if (MouseRight.Clicked)
                FocusManager.SetFocused(frame);
        }
    }

    public class GainFocusOnAnyMouseInput : FocusUpdater
    {
        public override void UpdateFocus(Frame frame)
        {
            if (!frame.CursorWithin)
                return;
            if (MouseLeft.Clicked || MouseMiddle.Clicked || MouseRight.Clicked || ScrolledUp || ScrolledDown || ScrolledLeft || ScrolledRight)
                FocusManager.SetFocused(frame);
        }
    }
}