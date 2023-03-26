
using Microsoft.Xna.Framework;

namespace Xylem.Framework
{
    public static class FocusManager
    {
        private static Frame _focusedFrame = null;

        public static Frame Focused => _focusedFrame;

        public static void SetFocused(Frame frame)
        {
            if (_focusedFrame != null)
                _focusedFrame.Focused = false;
            
            if (frame != null)
                frame.Focused = true;

            _focusedFrame = frame;
        }

        public static void SetUnfocused(Frame frame)
        {
            frame.Focused = false;

            if (frame.Equals(_focusedFrame))
            {
                SetFocused(null);
                return;
            }
        }

        public static void ReceiveTextInput(object sender, TextInputEventArgs args)
        {
            if (_focusedFrame != null)
                _focusedFrame.ReceiveTextInput(args);
        }
    }
}