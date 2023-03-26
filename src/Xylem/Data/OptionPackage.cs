
using System;
using System.Text.Json;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xylem.Registration;
using Xylem.Component;
using Xylem.Reference;
using Xylem.Reflection;

namespace Xylem.Data
{
    public abstract class OptionEntry<V>
    {
        public readonly string Identifier;
        public V Value { get; set; }

        protected OptionEntry(string identifier, V defaultValue)
        {
            Identifier = identifier;

            if (defaultValue == null)
                throw new ArgumentNullException("Default option values cannot be null");

            Value = defaultValue;
        }

        public abstract void Import(JObject data);
        public abstract void Export(JObject data);
    }

    // Summary:
    //      This registry is a modification of the base registry that manages option
    //      importing. Its operation is fundamentally different when compared to most
    //      registries, since its entries are not defined by the 'entries' token of the
    //      JSON data files that this registry will receive. Instead, each individual
    //      JSON data file received by this registry represents a separate options
    //      package. Each entry defines a key for a specific option, as well as the
    //      current value for that option. 
    public sealed class OptionDataPackageRegistry : Registry<OptionDataPackage>
    {
        public OptionDataPackageRegistry(string folder, string key) : base(folder, key) {}

        protected override IEnumerable<string> ImportJson(JObject data)
        {
            VerifyType(data);

            string identifier = (string) data[K.Identifier];
            JObject entries = data[K.Entries] as JObject;
            OptionDataPackage package = new OptionDataPackage(identifier);

            foreach (var entry in entries)
                package[entry.Key] = entry.Value;

            Register(package);

            yield return null;
        }
    }

    public sealed class OptionDataPackage : RegistryEntry, IEnumerable<KeyValuePair<string, JToken>>
    {
        private readonly Dictionary<string, JToken> _dataEntries;

        internal OptionDataPackage(string identifier)
        {
            Identifier = identifier;
            _dataEntries = new Dictionary<string, JToken>();
        }

        internal JToken this[string key]
        {
            get => _dataEntries[key];
            set => _dataEntries[key] = value;
        }

        public IEnumerator<KeyValuePair<string, JToken>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, JToken>>)_dataEntries).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => _dataEntries.GetEnumerator();
    }

    public abstract class OptionPackage
    {
        public static V Import<V>(string dataPackageKey) where V : OptionPackage
        {
            if (!R.Options.Has(dataPackageKey))
                throw new KeyNotFoundException($"An option data package is not registered for identifier '{dataPackageKey}'");

            OptionDataPackage dataPackage = R.Options[dataPackageKey];

            Type packageType = typeof(V);

            V package = Introspector.Instantiate<V>(packageType, typeof(OptionPackage));

            foreach (var entry in dataPackage)
                package.ImportEntry(entry.Key, entry.Value);

            return package;
        }

        protected OptionPackage() {}

        protected abstract void ImportEntry(string key, JToken token);
        protected abstract IEnumerable<KeyValuePair<string, JToken>> ExportEntries();

        public abstract bool Has(string key);
    }

    public class ValueOptionPackage : OptionPackage
    {
        private readonly Dictionary<string, object> _entries;

        public ValueOptionPackage()
        {
            _entries = new Dictionary<string, object>();
        }

        public override bool Has(string key) => _entries.ContainsKey(key);

        public V Get<V>(string key, V defaultValue = default(V))
        {
            if (!Has(key))
                return default(V);
            
            try 
            {
                return (V) _entries[key];
            }

            catch (InvalidCastException e)
            {
                Output.Suggest($"Failed to cast generic value from option package: '{typeof(V)}'");
                Output.Suggest(e.StackTrace);

                return defaultValue;
            }
        }

        protected override IEnumerable<KeyValuePair<string, JToken>> ExportEntries()
        {
            throw new NotImplementedException();
        }

        protected override void ImportEntry(string key, JToken token)
        {
            if (token.Type == JTokenType.Integer)
                _entries[key] = (int) token;

            else if (token.Type == JTokenType.String)
                _entries[key] = (string) token;

            else if (token.Type == JTokenType.Float)
                _entries[key] = (float) token;
            
            else if (token.Type == JTokenType.Boolean)
                _entries[key] = (bool) token;

            else 
                Output.Suggest($"Failed to import option entry: '[{key}:{token}]'");
        }
    }

    public static class Shortcuts
    {
        public static Keys UIShowTooltips { get; private set; } = Keys.LeftShift;
    }
}