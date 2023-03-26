
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Input;
using Xylem.Graphics;
using Xylem.Reflection;
using Xylem.Functional;
using Xylem.Registration;
using Xylem.Reference;

namespace Xylem.Interaction
{
    public abstract class InteractionEntry : RegistryEntry
    {
        public static V Import<V>(JObject data) where V : InteractionEntry
        {
            string className = J.ReadString(data, K.Class);

            V entry = Introspector.Instantiate<V>(className, typeof(V), data);

            return entry;
        }

        protected InteractionEntry(JObject data) : base(data) 
        {
            Initialize();
        }

        protected virtual void Initialize() {}
    }

    public sealed class InteractionRegistry<V> : Registry<V> where V : InteractionEntry
    {
        public InteractionRegistry(string folder, string key) : base(folder, key)
        {

        }

        protected override void ImportEntryJson(JObject data)
        {
            Register(InteractionEntry.Import<V>(data));
        }
    }

    /**
     * Causes are the root of interactions. The conditions for the occurrence of an
     * interaction are determined by its cause, and that cause is responsible for invoking
     * the appropriate effects. 
     */
    public abstract class Cause : InteractionEntry
    {
        protected readonly HashSet<string> EffectKeys = new HashSet<string>();

        protected Cause(JObject data) : base(data) 
        {
            J.ReadArrayTokens(data, K.Effects, tok => EffectKeys.Add((string) tok));
        }

        /**
         * Invokes any effects or other behaviors associated with this cause, as long as
         * all of the conditions for this cause are met when queried.
         *
         * This sequence is optional, and best used for causes that are based on
         * continuous querying. Providing the Update function of a cause to the
         * UpdateDispatcher within the Initialization of that cause will ensure that it is
         * continuously queried. 
         */
        protected void Update()
        {
            if (Query())
            {
                InvokeEffects();
                Invoke();
            }
        }

        /**
         * Invokes all of the effects derived from the data entry of this cause. 
         */
        protected void InvokeEffects()
        {
            foreach (string effectKey in EffectKeys)
            {
                if (R.Effects.Has(effectKey))
                    R.Effects[effectKey].Invoke();
                else 
                    Output.Suggest($"An effect is not registered for identifier '{effectKey}'");
            }
        }

        /**
         * Queries the conditions of this cause. 
         *
         * Returns true when all conditions are met, and false otherwise.
         */
        protected virtual bool Query() => false;

        /**
         * Invoke any specialized consequences of a successful query. 
         */
        protected virtual void Invoke() {}
    }

    /**
     * Effects are the branches of interactions. The definition of effects enables
     * specific effects to be reused across multiple contexts, including within multiple
     * distinct causes. Effects may also be used to invoke other effects, but should not
     * invoke themselves.
     */
    public abstract class Effect : InteractionEntry
    {
        protected Effect(JObject data) : base(data) {}

        /**
         * 
         */
        public virtual void Invoke() {}
    }

    public class DebugEffect : Effect
    {
        private readonly string _text;

        public DebugEffect(JObject data) : base(data)
        {
            _text = J.ReadString(data, K.Value);
        }

        public override void Invoke()
        {
            UpdateDispatcher.EnqueueRenderAction(() => Text.RenderTextAt(R.Typefaces["senui"], _text, 1000, 90, 3));

            UpdateDispatcher.EnqueueUpdate(() => R.InputBindings["debug_display_text"].KeyCode = Keys.N);
        }
    }
}