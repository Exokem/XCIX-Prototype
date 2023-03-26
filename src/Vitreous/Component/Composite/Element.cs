
using Newtonsoft.Json.Linq;
using Xylem.Reference;
using Vitreous.Reference;
using Vitreous.Registration;

namespace Vitreous.Component.Composite
{
    /**
     * Elements are irrevokably the whole in the part-whole relationship between
     * elements and parts. An element may be composed of parts, and parts may be composed
     * of parts, but parts may never be composed of elements.
     */
    public class ElementEntry : CompositeEntry
    {
        public static ElementEntry Empty => Registries.Elements[K.Empty];

        // Are instances of this entry distinct (not identical and not interchangeable)
        public readonly bool Distinct;

        public ElementEntry(JObject data) : base(data)
        {
            Distinct = J.ReadBool(data, VK.Distinct, false);
        }
    }

    public class Element : CompositeInstance<ElementEntry>
    {
        public Element(ElementEntry element) : base(element)
        {
        }

        public Element(JObject data) : base(data, Registries.Elements) 
        {
            // Read extra data into fields as needed
        }
    }
}