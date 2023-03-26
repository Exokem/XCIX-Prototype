
using System;
using System.Collections.Generic;
using Xylem.Functional;

namespace Xylem.Input
{
    public enum InputStateValue { Pressed, Released, Clicked }

    public delegate void StateListener();

    public sealed class InputState
    {
        /* Is the input pressed */
        public bool Pressed { get; private set; } = false;
        
        /* Was the input just released */
        public bool Released { get; private set; } = false;

        /* Was the input just depressed */
        public bool Clicked { get; private set; } = false;

        /* Was the input pressed before the last update */
        public bool WasPressed { get; private set; } = false;

        /* Was the input released before the last update */
        public bool WasReleased { get; private set; } = false;

        /* Was the input clicked before the last update */
        public bool WasClicked { get; private set; } = false;

        private readonly Dictionary<InputStateValue, HashSet<StateListener>> _listeners = new Dictionary<InputStateValue, HashSet<StateListener>>();

        /* When was this input last clicked */
        public DateTime ClickedTime { get; private set; }

        private DateTime _repeatReference;
        private bool _repeating;

        public bool RepeatClicked { get; private set; } = false;

        internal InputState() {}

        public void Update(bool pressed)
        {
            WasPressed = Pressed;
            Pressed = pressed;

            // The input is released if it was depressed before but is now
            WasReleased = Released;
            Released = WasPressed && !Pressed;

            // The input is clicked if it is depressed now but wasn't before
            WasClicked = Clicked;
            Clicked = Pressed && !WasPressed;

            if (Clicked)
            {
                ClickedTime = DateTime.Now;
                _repeatReference = DateTime.Now;
                RepeatClicked = false;
            }

            else if (Pressed)
            {
                if (!_repeating)
                {
                    _repeating = HeldFor(0.5D, ref _repeatReference);
                    RepeatClicked = false;
                }
                
                else RepeatClicked = HeldFor(0.03, ref _repeatReference);
            }

            else 
            {
                _repeating = false;
                RepeatClicked = false;
            }

            if (Clicked && _listeners.ContainsKey(InputStateValue.Clicked))
            {
                foreach (StateListener listener in _listeners[InputStateValue.Clicked])
                    listener();
            }

            else if (Pressed && _listeners.ContainsKey(InputStateValue.Pressed))
            {
                foreach (StateListener listener in _listeners[InputStateValue.Pressed])
                    listener();
            }

            else if (Released && _listeners.ContainsKey(InputStateValue.Released))
            {
                foreach (StateListener listener in _listeners[InputStateValue.Released])
                    listener();
            }
        }

        public bool HeldFor(double seconds, ref DateTime reference)
        {
            double secondsHeld = DateTime.Now.Subtract(reference).TotalSeconds;
            if (seconds <= secondsHeld)
            {
                // Console.WriteLine(secondsHeld);
                reference = DateTime.Now;
                return true;
            }
            else return false;
        }

        public void Listen(InputStateValue target, StateListener receiver)
        {
            HashSet<StateListener> listeners = _listeners.GetOrDefault(target, new HashSet<StateListener>());

            listeners.Add(receiver);

            _listeners[target] = listeners;
        }

        public void Ignore(InputStateValue target, StateListener receiver)
        {
            if (!_listeners.ContainsKey(target))
                return;
            
            _listeners[target].Remove(receiver);
        }

        public TimeSpan PressDuration() => DateTime.Now.Subtract(ClickedTime);

        public void PrintDebugInfo()
        {
            Output.Write($"P: {Pressed} C: {Clicked} R: {Released} rep: {_repeating}");
        }

        public override string ToString()
        {
            return $"P: {Pressed} C: {Clicked} R: {Released} rep: {_repeating}";
        }
    }
}