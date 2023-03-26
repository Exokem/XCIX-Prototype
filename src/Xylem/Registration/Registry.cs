
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System;
using Xylem.Data;
using Xylem.Functional;
using Xylem.Component;
using Xylem.Reference;

namespace Xylem.Registration
{
    public abstract class RegistryEntry : NamedComponent
    {
        protected RegistryEntry() {}

        protected RegistryEntry(JObject data) : base(data) {}

        public virtual void Inherit<V>(List<V> inheritanceTargets) where V : RegistryEntry
        {

        }
    }

    public class RegistrySet : IndexedMap<string, Registry>
    {
        protected override string GetKeyForItem(Registry item)
        {
            return item.Key;
        }
    }

    public abstract class Registry
    {
        private static readonly RegistrySet _registries = new RegistrySet();

        public static IEnumerable<Registry> Registries()
        {
            for (int i = 0; i < _registries.Count; i ++)
            {
                yield return _registries.Get(i);
            }
        }

        public readonly string Folder;
        public readonly string Key;

        public FileReceiver ImportFile { get; protected set; }
        public DirectoryReceiver ImportDirectory { get; protected set; }

        public abstract int Size { get; }

        protected Registry(string folder, string key)
        {
            Key = key;
            Folder = folder;

            _registries.Add(this);
        }

        protected void ImportDataFile(FileInfo file)
        {
            if (file.Extension != ".json" && file.Extension != ".jsonc")
                return;

            try
            {
                JObject data = JObject.Parse(File.ReadAllText(file.FullName));
                foreach (string error in ImportJson(data))
                {
                    if (error != null)
                        Output.Suggest($"{file.Name} - Skipping entry: {error}");
                }
            }

            catch (Exception e)
            {
                Output.Suggest($"Skipping import '{file.FullName}': {e.Message}\n{e.StackTrace}");
            }
        }

        protected abstract IEnumerable<string> ImportJson(JObject data);
    }

    /**
     * The path of a registry corresponds mainly to the type of its entries when their
     * data files are imported. By default, the import order of data files is based on
     * usage precedence. For example, the attribute registry will be populated from the
     * Attributes directory first.
     */
    public class Registry<V> : Registry where V : RegistryEntry
    {
        protected readonly Dictionary<string, V> _entries = new Dictionary<string, V>();

        public Registry
        (
            string folder, string key
        ) : base(folder, key) 
        {
            ImportFile = ImportDataFile;
            ImportDirectory = Importer.ImportRecursive;
        }

        public void Register(V entry)
        {
            if (entry == null)
                return;

            _entries[entry.Identifier] = entry;
        }

        public V this[string identifier] => Has(identifier) ? _entries[identifier] : null;

        public V GetOrDefault(string identifier, V defaultEntry)
        {
            return Has(identifier) ? this[identifier] : defaultEntry;
        }

        public bool Has(string identifier) => identifier != null && _entries.ContainsKey(identifier);

        public override int Size => _entries.Count;

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        protected void VerifyType(JObject data)
        {
            string type = (string) data[K.Type];
            if (type != Key)
                throw new JsonException($"Type '{type}' does not match registry '{Key}'");
        }

        protected override IEnumerable<string> ImportJson(JObject data)
        {
            VerifyType(data);

            JArray entries = data[K.Entries] as JArray;

            foreach (var entry in entries)
            {
                string error = null;

                try 
                {
                    ImportEntryToken(entry);
                }

                catch (Exception e)
                {
                    error = e.StackTrace;
                }

                if (error != null)
                    yield return error;
            }
        }

        protected virtual void ImportEntryToken(JToken token)
        {
            if (token is JObject entryData)
                ImportEntryJson(entryData);
        }

        protected virtual void ImportEntryJson(JObject data)
        {
            List<V> inheritanceTargets = new List<V>();

            // Retrieve Inherited Descriptors
            J.ReadArrayStrings(data, K.Inherits, idn => 
            {
                inheritanceTargets.Add(this[idn]);
            });

            V instance = (V) Activator.CreateInstance(typeof(V), data);
            instance.Type = Key;
            instance.Inherit<V>(inheritanceTargets);

            Register(instance);
        }

        public IEnumerable<V> Entries() => _entries.Values;
    }
}