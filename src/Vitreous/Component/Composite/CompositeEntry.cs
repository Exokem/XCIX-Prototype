
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Functional;
using Xylem.Reference;
using Xylem.Registration;
using Vitreous.Registration;
using Vitreous.Reference;

namespace Vitreous.Component.Composite
{
    /**
     * CompositeEntry is an abstract root in the hierarchy of program objects. It exists 
     * as a common ancestor to Element and Part because the two classes share the same
     * fundamental structure, but differ for important semantic reasons. This
     * abstraction simplifies the definition of their common behaviors while keeping them
     * from being interchangeable in specific cases.
     */
    public abstract class CompositeEntry : DescribedEntry
    {
        public readonly Dictionary<string, int> Parts = new Dictionary<string, int>();

        protected CompositeEntry(JObject data) : base(data)
        {
            // Store Part Information - No instantiation occurs here
            J.ReadArray(data, VK.Parts, obj => 
            {
                string partIdn = J.ReadString(obj, K.Identifier);
                int count = J.ReadInt(obj, K.Count, 1);

                Parts[partIdn] = count;
            });
        }
    }

    public class CompositeInstance<V> : DescribedInstance<V> where V : CompositeEntry
    {
        public readonly OrderedSet<Part> PartSet = new OrderedSet<Part>();

        /**
         * Produces a CompositeInstance based on a registered CompositeEntry. This simply 
         * involves instantiating everything defined within the supplied reference, or
         * copying them if they are already defined.
         */
        protected CompositeInstance(V compositeEntry) : base(compositeEntry)
        {
            // Instantiate parts n times
            foreach ((string key, int value) in Reference.Parts)
            {
                foreach (var n in Enumerable.Range(0, value))
                    PartSet.Add(new Part(Registries.Parts[key]));
            }
        }

        protected CompositeInstance(JObject data, Registry<V> registry) : base(data, registry)
        {
            J.ReadArray(data, VK.Parts, obj => PartSet.Add(new Part(obj)));
        }

        public override void Export(JObject data)
        {
            base.Export(data);
            data[VK.Parts] = J.WriteArray(PartSet);
        }
    }
}