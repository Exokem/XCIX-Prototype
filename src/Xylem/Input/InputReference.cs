
using Microsoft.Xna.Framework.Input;

namespace Xylem.Input
{
    public static class I
    {
        public static readonly InputClaim CONTROL_Z = new InputClaim(Keys.Z, clicked: true, control: true, repeated: true);

        public static readonly InputClaim CONTROL_Y = new InputClaim(Keys.Y, clicked: true, control: true, repeated: true);

        public static readonly InputClaim CONTROL_S = new InputClaim(Keys.S, clicked: true, control: true);

        public static readonly InputClaim MIDDLE_MOUSE_PRESSED = 
            new InputClaim(InputProcessor.MouseMiddle, pressed: true, isKey: false);

        public static readonly BoolClaim ALT_MODIFIER = 
            new BoolClaim(() => true, alt: true);
        public static readonly BoolClaim CONTROL_MODIFIER = 
            new BoolClaim(() => true, control: true);
        public static readonly BoolClaim SHIFT_MODIFIER = 
            new BoolClaim(() => true, shift: true);

        public static readonly BoolClaim CONTROL_SHIFT_MODIFIER = 
            new BoolClaim(() => true, control: true, shift: true);

        public static readonly BoolClaim ALT_CONTROL_MODIFIER = 
            new BoolClaim(() => true, alt: true, control: true);
        public static readonly BoolClaim ALT_SHIFT_MODIFIER = 
            new BoolClaim(() => true, alt: true, shift: true);

        public static readonly BoolClaim ALT_CONTROL_SHIFT_MODIFIER = 
            new BoolClaim(() => true, alt: true, control: true, shift: true);

        public static readonly BoolClaim SCROLL_UP = 
            new BoolClaim(() => InputProcessor.ScrolledUp) {ExactModifiers = false};
        public static readonly BoolClaim SCROLL_RIGHT = 
            new BoolClaim(() => InputProcessor.ScrolledRight) {ExactModifiers = false};
        public static readonly BoolClaim SCROLL_DOWN = 
            new BoolClaim(() => InputProcessor.ScrolledDown) {ExactModifiers = false};
        public static readonly BoolClaim SCROLL_LEFT = 
            new BoolClaim(() => InputProcessor.ScrolledLeft) {ExactModifiers = false};

        public static readonly BoolClaim SHIFT_SCROLL_UP = 
            new BoolClaim(() => InputProcessor.ScrolledUp, shift: true);
        public static readonly BoolClaim SHIFT_SCROLL_RIGHT = 
            new BoolClaim(() => InputProcessor.ScrolledRight, shift: true);
        public static readonly BoolClaim SHIFT_SCROLL_DOWN = 
            new BoolClaim(() => InputProcessor.ScrolledDown, shift: true);
        public static readonly BoolClaim SHIFT_SCROLL_LEFT = 
            new BoolClaim(() => InputProcessor.ScrolledLeft, shift: true);
    }
}