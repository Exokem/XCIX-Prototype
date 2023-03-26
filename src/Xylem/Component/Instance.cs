
using Newtonsoft.Json.Linq;
using Xylem.Functional;
using Xylem.Registration;
using Xylem.Reference;

namespace Xylem.Component
{
    public interface IDuplicable<V>
    {
        public V Duplicate();
    }

    public abstract class AbstractInstance<V> : JsonComposite, IIdentifiable where V : RegistryEntry
    {
        public readonly V Reference;

        public string Identifier => Reference.Identifier;

        protected AbstractInstance(V registryEntry)
        {
            Reference = registryEntry;
        }

        protected AbstractInstance(JObject data, Registry<V> registry)
        {
            Reference = registry[J.ReadString(data, K.Reference)];
        }

        public override void Export(JObject data)
        {
            // JObject referenceData = new JObject();
            // Reference.Export(referenceData);
            // data[Reference.Type] = referenceData;
            data[K.Reference] = Identifier;
        }

        // public override bool Equals(object obj)
        // {
        //     if (obj is AbstractInstance<V> inst)
        //         return inst.Reference == Reference;

        //     return false;
        // }

        // public override int GetHashCode()
        // {
        //     return Reference.GetHashCode();
        // }
    }
}
