
using Newtonsoft.Json.Linq;
using Xylem.Reference;

using Vitreous.Registration;

namespace Vitreous.Component.Composite
{
    /**
     * Parts are the building blocks of elements. The relationship between elements and
     * parts is most appropriately described as composition, where an element is a
     * composition of its parts. This is mostly semantic and nontechnical in meaning, as
     * part objects may not necessarily be destroyed along with their whole elements, but
     * in the context of the program, a part will never exist outside of an element or
     * another part.
     *
     * A hand is composed of fingers; if the hand is destroyed, so are the fingers. A
     * finger may exist in isolation within an element whose only part is that single
     * finger.
     */
    public class PartEntry : CompositeEntry
    {
        public static PartEntry Empty => Registries.Parts[K.Empty];

        public PartEntry(JObject data) : base(data)
        {

        }
    }

    public class Part : CompositeInstance<PartEntry>
    {
        public Part(PartEntry part) : base(part)
        {
        }

        public Part(JObject data) : base(data, Registries.Parts)
        {

        }
    }
}