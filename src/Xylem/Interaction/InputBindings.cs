
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Input;
using Xylem.Registration;
using Xylem.Reference;
using Xylem.Input;
using Xylem.Functional;
using Xylem.Reflection;

namespace Xylem.Interaction
{
    /**
     * Input Bindings represent specific control bindings. An example is a 'jump' input
     * binding. 
     *
     * Input Bindings are intended to specify what kinds of actions can be controlled
     * by the user's input, and to centralize references to those actions. Each input
     * binding is associated with an Input Binding Cause, which manages the effects that
     * occur based on the state of the key for the binding. 
     */
    public class InputBindingEntry : RegistryEntry
    {
        private readonly HashSet<Receiver<Keys>> _receivers = new HashSet<Receiver<Keys>>();
        private readonly InputBindingCause _cause;

        // Bindings are unbound by default
        private Keys _keyCode = Keys.None;
        public Keys KeyCode 
        {
            get => _keyCode;
            set 
            {
                _cause?.RebindListeners(_keyCode, value);
                _keyCode = value;
            }
        }

        public InputBindingEntry(JObject data) : base(data)
        {
            string causeClass = J.ReadString(data, K.Cause, null);

            if (causeClass != null) 
                _cause = Introspector.Instantiate<InputBindingCause>(causeClass, typeof(InputBindingCause), data);
            else 
                _cause = new InputBindingCause(data);

            if (data.ContainsKey(K.Key))
                KeyCode = (Keys) J.ReadInt(data, K.Key);
        }

        public void AddRebindReceiver(Receiver<Keys> receiver) => _receivers.Add(receiver);
    }

    /**
     * Input Binding Causes are specialized to managing the triggers for input bindings. 
     * Each such cause is attached to a specific input binding, and can be adjusted to 
     * listen to different Input States if the Input Binding to which it belongs is 
     * rebound to a different key. 
     */
    public class InputBindingCause : Cause
    {
        protected readonly Effect OnClickedEffect, OnPressedEffect, OnReleasedEffect;

        public InputBindingCause(JObject data) : base(data) 
        {
            if (data.ContainsKey(K.Effects) && data[K.Effects] is JObject effectData)
            {
                OnClickedEffect = R.Effects[J.ReadString(effectData, K.ClickEffect, null)];
                OnPressedEffect = R.Effects[J.ReadString(effectData, K.PressEffect, null)];
                OnReleasedEffect = R.Effects[J.ReadString(effectData, K.ReleaseEffect, null)];
            }
        }

        /**
         * Rebinds the listeners of this cause to the specified key.
         */
        internal void RebindListeners(Keys previous, Keys next)
        {
            if (previous == next)
                return;

            if (previous != Keys.None)
            {
                InputProcessor.Ignore(previous, InputStateValue.Clicked, OnClicked);
                InputProcessor.Ignore(previous, InputStateValue.Pressed, OnPressed);
                InputProcessor.Ignore(previous, InputStateValue.Released, OnReleased);
            }

            InputProcessor.Listen(next, InputStateValue.Clicked, OnClicked);
            InputProcessor.Listen(next, InputStateValue.Pressed, OnPressed);
            InputProcessor.Listen(next, InputStateValue.Released, OnReleased);
        }

        protected virtual void OnClicked() => OnClickedEffect?.Invoke();

        protected virtual void OnPressed() => OnPressedEffect?.Invoke();

        protected virtual void OnReleased() => OnReleasedEffect?.Invoke();
    }
}