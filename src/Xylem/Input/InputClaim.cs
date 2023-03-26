
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Xylem.Functional;

using static System.HashCode;
using static Xylem.Input.InputProcessor;


namespace Xylem.Input
{
    public abstract class AbstractClaim
    {
        protected readonly bool Alt;
        protected readonly bool Control;
        protected readonly bool Shift;

        protected readonly int KeysDown;

        protected bool Claimed { get; private set; }

        public bool ExactModifiers { get; set; } = false;

        protected virtual bool AltPressed => 
            InputProcessor.Pressed(Keys.LeftAlt) || InputProcessor.Pressed(Keys.RightAlt);
        protected virtual bool ControlPressed =>
            InputProcessor.Pressed(Keys.LeftControl) || InputProcessor.Pressed(Keys.RightControl);
        protected virtual bool ShiftPressed =>
            InputProcessor.Pressed(Keys.LeftShift) || InputProcessor.Pressed(Keys.RightShift);

        protected AbstractClaim(bool alt, bool control, bool shift, int baseKeysDown = 1)
        {
            Alt = alt;
            Control = control;
            Shift = shift;

            int keysDown = baseKeysDown;

            if (Alt)   
                keysDown ++;
            if (Control)
                keysDown ++;
            if (Shift)
                keysDown ++;

            KeysDown = keysDown;

            Claimed = false;
        }

        protected abstract bool InternalQuery();

        public bool Query()
        {
            if (Claimed)
                return false;

            if (!InternalQuery())
                return false;

            if (ExactModifiers)
            {
                if (Alt != AltPressed || Control != ControlPressed || Shift != ShiftPressed)
                    return false;
            }

            else 
            {
                if (Alt && !AltPressed || Control && !ControlPressed || Shift && !ShiftPressed)
                    return false;
            }

            return true;
        }

        public bool RequestClaim()
        {
            if (Query())
            {
                Claimed = true;

                UpdateDispatcher.EnqueueUpdate(() => Claimed = false);

                return true;
            }

            else 
                return false;
        }
    }

    public class BoolClaim : AbstractClaim
    {
        private readonly Provider<bool> _stateProvider;

        public BoolClaim(Provider<bool> stateProvider, bool alt = false, bool control = false, bool shift = false) : base(alt, control, shift, baseKeysDown: 0)
        {
            _stateProvider = stateProvider;
        }

        protected override bool InternalQuery() => _stateProvider();
    }

    public class InputClaim : AbstractClaim
    {
        private readonly InputState _state;

        private readonly bool _clicked;
        private readonly bool _pressed;
        private readonly bool _released;
        private readonly bool _repeated;

        public InputClaim
        (
            Keys key, 
            bool alt = false, bool control = false, bool shift = false, 
            bool clicked = false, bool pressed = false, bool released = false, 
            bool repeated = false
        ) : this(State(key), alt, control, shift, clicked, pressed, released, repeated) {}

        public InputClaim
        (
            InputState state, 
            bool alt = false, bool control = false, bool shift = false, 
            bool clicked = false, bool pressed = false, bool released = false, 
            bool repeated = false, bool isKey = true
        ) : base(alt, control, shift, isKey ? 1 : 0)
        {
            _state = state;

            _clicked = clicked;
            _pressed = pressed;
            _released = released;
            _repeated = repeated;
        }

        protected virtual bool Clicked => _state.Clicked;
        protected virtual bool Pressed => _state.Pressed;
        protected virtual bool Released => _state.Released;
        protected virtual bool Repeated => _state.RepeatClicked;

        protected override bool InternalQuery()
        {
            if (_clicked && !Clicked && (!_repeated || (_repeated && !Repeated)))
                return false;

            if (_pressed && !Pressed)
                return false;
            
            if (_released && !Released)
                return false;

            return true;
        }
    }
}