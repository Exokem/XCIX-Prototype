using Newtonsoft.Json.Linq;
using Xylem.Functional;
using Xylem.Reference;

namespace Xylem.Component
{
    public abstract class NamedComponent : JsonComposite, IIdentifiable
    {
        // Conventionally unique identifier (not enforced)
        public string Identifier { get; internal set; }
        public string Description { get; protected set; }

        internal string Type;

        protected NamedComponent() {}

        protected NamedComponent(JObject data)
        {
            Identifier = J.ReadString(data, K.Identifier);
            Description = J.ReadString(data, K.Description, "");
        }

        //
        // Summary:
        //      Exports the content of this component into the provided JSON data object.
        public override void Export(JObject data)
        {
            data[K.Identifier] = Identifier;
            if (Description != null && Description.Length != 0)
                data[K.Description] = Description;
        }

        // public override bool Equals(object obj)
        // {
        //     if (obj is NamedComponent comp)
        //         return comp.Identifier == Identifier;

        //     return false;
        // }

        // public override int GetHashCode()
        // {
        //     return Identifier.GetHashCode();
        // }
    }
}