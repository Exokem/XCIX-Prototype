
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Registration;
using Xylem.Reference;
using Xylem.Component;
using Xylem.Functional;
using Vitreous.Registration;

namespace Vitreous.Component.Core
{
    public class StateEntry : RegistryEntry
    {
        private readonly OrderedSet<StateValue> _values = new OrderedSet<StateValue>();
        // private readonly EntryMap<StateValue> _values = new EntryMap<StateValue>();
        public StateValue BaseValue { get; private set; }

        public StateEntry(JObject data) : base(data)
        {
            string baseValueKey = J.ReadString(data, K.BaseValue);

            J.ReadArray(data, K.Values, obj => 
            {
                StateValue value = new StateValue(obj);
                _values.Add(value);

                if (value.Identifier == baseValueKey)
                    BaseValue = value;
            });
        }

        public bool HasValue(string valueKey) => _values.Contains(valueKey);

        public StateValue this [string valueKey]
        {
            get => _values[valueKey];
        }

        public IEnumerable<StateValue> Values()
        {
            foreach (StateValue value in _values)
                yield return value;
        }
    }

    public class StateValue : NamedComponent
    {
        public StateValue(JObject data) : base(data) {}
    }

    public class State : AbstractInstance<StateEntry>, IDuplicable<State>
    {
        public StateValue Value { get; set; }

        public State(JObject data) : base(data, Registries.States)
        {
            Value = Reference[J.ReadString(data, K.Value)];
        }

        public State(StateEntry state) : base(state) 
        {
            Value = Reference.BaseValue;
        }

        public override void Export(JObject data)
        {
            base.Export(data);
            data[K.Value] = Value.Identifier;
        }

        public State Duplicate()
        {
            State duplicate = new State(Reference);
            duplicate.Value = Value;
            return duplicate;
        }

        public void ImportExtraData(JObject data)
        {
            string valueKey = J.ReadString(data, K.Value, null);

            if (valueKey != null && Reference.HasValue(valueKey))
                Value = Reference[valueKey];
        }

        // public override bool Equals(object obj)
        // {
        //     if (base.Equals(obj) && obj is State state)
        //         return state.Value == Value;

        //     return false;
        // }

        // public override int GetHashCode()
        // {
        //     return System.HashCode.Combine(base.GetHashCode(), Value.GetHashCode());
        // }
    }
}