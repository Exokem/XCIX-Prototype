
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Xylem.Graphics;
using Xylem.Functional;

namespace Xylem.Input
{
    public sealed class InputProcessor
    {
        public static readonly InputState MouseLeft = new InputState();
        public static readonly InputState MouseMiddle = new InputState();
        public static readonly InputState MouseRight = new InputState();

        // private static int _x, _y;

        public static int X => Mouse.GetState().X + (GraphicsConfiguration.Borderless ? GraphicsContext.X : 0);
        public static int Y => Mouse.GetState().Y + (GraphicsConfiguration.Borderless ? GraphicsContext.Y : 0);

        // Middle Mouse Position Differentials
        private static int _lastMX, _lastMY;
        public static int MDX => X - _lastMX;
        public static int MDY => Y - _lastMY;

        public static bool HasMouseMoved => MDX != 0 || MDY != 0;

        public static bool ScrolledUp { get; private set; }
        public static bool ScrolledDown { get; private set; }

        public static bool ScrolledLeft { get; private set; }
        public static bool ScrolledRight { get; private set; }

        public static int SD => _scrollValue - _lastScrollValue;

        private static int _lastScrollValue = Mouse.GetState().ScrollWheelValue;
        private static int _scrollValue = Mouse.GetState().ScrollWheelValue;
        
        private static int _lastHScrollValue = Mouse.GetState().HorizontalScrollWheelValue;
        private static int _hScrollValue = Mouse.GetState().HorizontalScrollWheelValue;

        public static int KeysPressed { get; private set; }

        private static readonly Dictionary<Keys, InputState> _keys = new Dictionary<Keys, InputState>();

        public static bool AnyMouseClicked => MouseLeft.Clicked || MouseMiddle.Clicked || MouseRight.Clicked;

        public static void Update()
        {
            MouseState cursor = Mouse.GetState();
            MouseLeft.Update(cursor.LeftButton == ButtonState.Pressed);
            MouseMiddle.Update(cursor.MiddleButton == ButtonState.Pressed);
            MouseRight.Update(cursor.RightButton == ButtonState.Pressed);

            

            // if (MouseMiddle.Clicked)
            // {
                
            // }

            KeyboardState keyboard = Keyboard.GetState();

            KeysPressed = keyboard.GetPressedKeyCount();

            foreach (Keys key in keyboard.GetPressedKeys())
            {
                if (!_keys.ContainsKey(key))
                    _keys[key] = new InputState();
            }

            foreach ((Keys key, InputState state) in _keys)
            {
                if (key != Keys.None)
                    state.Update(keyboard.IsKeyDown(key));
            }

            UpdateScrollWheel();

            UpdateDispatcher.EnqueueUpdate(() => 
            {
                _lastMX = X;
                _lastMY = Y;
            });
        }

        // private static void UpdateMousePosition()
        // {
        //     int x = X, y = Y;

        //     int dx = x - _x;
        //     int dy = y - _y;


        // }

        private static void UpdateScrollWheel()
        {
            int value = Mouse.GetState().ScrollWheelValue;

            ScrolledUp = !ScrolledUp && (_lastScrollValue != value && _lastScrollValue < value);
            ScrolledDown = !ScrolledDown && (_lastScrollValue != value && value < _lastScrollValue);

            _lastScrollValue = _scrollValue;
            _scrollValue = value;

            int hValue = Mouse.GetState().HorizontalScrollWheelValue;

            ScrolledLeft = _lastHScrollValue != hValue && _lastHScrollValue < hValue;
            ScrolledRight = _lastHScrollValue != hValue && hValue < _lastHScrollValue;

            _lastHScrollValue = _hScrollValue;
            _hScrollValue = hValue;
        }

        public static bool Clicked(Keys key) => _keys.ContainsKey(key) && _keys[key].Clicked;

        public static bool Pressed(Keys key) => _keys.ContainsKey(key) && _keys[key].Pressed;

        public static bool Released(Keys key) => _keys.ContainsKey(key) && _keys[key].Released;

        public static bool RepeatClicked(Keys key) => _keys.ContainsKey(key) && _keys[key].RepeatClicked;

        public static void Listen(Keys key, InputStateValue target, StateListener receiver) => State(key).Listen(target, receiver);

        public static void Ignore(Keys key, InputStateValue target, StateListener receiver) => State(key).Ignore(target, receiver);

        public static void PrintDebugInfo(Keys key)
        {
            if (_keys.ContainsKey(key))
                _keys[key].PrintDebugInfo();
        }

        public static bool HeldFor(Keys key, double seconds, ref DateTime reference) =>
            _keys.ContainsKey(key) && _keys[key].HeldFor(seconds, ref reference);

        public static InputState State(Keys key) 
        {
            if (!_keys.ContainsKey(key))
                _keys[key] = new InputState();
            
            return _keys[key];
        }

        public static bool CursorWithin(Rectangle area)
        {
            return area.X <= X && X < area.Right && area.Y <= Y && Y < area.Bottom;
        }

        public static bool CursorWithinExclusive(Rectangle area)
        {
            return area.X <= X && X < area.Right && area.Y <= Y && Y < area.Bottom;
        }
    }
}