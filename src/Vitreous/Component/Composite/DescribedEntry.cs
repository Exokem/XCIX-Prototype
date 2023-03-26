
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Functional;
using Xylem.Component;
using Xylem.Reference;
using Xylem.Registration;
using Vitreous.Component.Core;
using Vitreous.Reference;

namespace Vitreous.Component.Composite
{
    public abstract class DescribedEntry : RegistryEntry
    {
        public readonly OrderedSet<Attribute> Attributes = new OrderedSet<Attribute>();
        public readonly OrderedSet<State> States = new OrderedSet<State>();
        public readonly OrderedSet<Qualifier> Qualifiers = new OrderedSet<Qualifier>();

        // public readonly InstanceMap<AttributeEntry, Attribute> Attributes = new InstanceMap< AttributeEntry, Attribute>();

        // public readonly InstanceMap<StateEntry, State> States = new InstanceMap<StateEntry, State>();

        // public readonly InstanceMap<QualifierEntry, Qualifier> Qualifiers = new InstanceMap<QualifierEntry, Qualifier>();

        // Construct a Described Entry from a JSON definition
        public DescribedEntry(JObject data) : base(data)
        {
            // Instantiate Attributes
            J.ReadArray(data, VK.Attributes, obj => Attributes.Add(new Attribute(obj)));

            // Instantiate States
            J.ReadArray(data, VK.States, obj => States.Add(new State(obj)));

            // Instantiate Qualifiers
            J.ReadArray(data, VK.Qualifiers, obj => Qualifiers.Add(new Qualifier(obj)));
        }

        public override void Inherit<V>(List<V> inheritanceTargets)
        {
            foreach (var target in inheritanceTargets)
            {
                if (target is DescribedEntry entry)
                {
                    foreach (var attribute in entry.Attributes)
                    {
                        if (!Attributes.Contains(attribute.Identifier))
                            Attributes.Add(attribute);
                    }

                    foreach (var state in entry.States)
                    {
                        if (!States.Contains(state.Identifier))
                            States.Add(state);
                    }

                    foreach (var qualifier in entry.Qualifiers)
                    {
                        if (!Qualifiers.Contains(qualifier.Identifier))
                            Qualifiers.Add(qualifier);
                    }
                }
            }
        }
    }

    public abstract class DescribedInstance<V> : AbstractInstance<V> where V : DescribedEntry
    {
        public readonly OrderedSet<Attribute> Attributes = new OrderedSet<Attribute>();
        public readonly OrderedSet<State> States = new OrderedSet<State>();
        public readonly OrderedSet<Qualifier> Qualifiers = new OrderedSet<Qualifier>();

        // public readonly InstanceMap<AttributeEntry, Attribute> Attributes = new InstanceMap<AttributeEntry, Attribute>();
        // public readonly InstanceMap<StateEntry, State> States = new InstanceMap<StateEntry, State>();
        // public readonly InstanceMap<QualifierEntry, Qualifier> Qualifiers = new InstanceMap<QualifierEntry, Qualifier>();

        // Create a Described Instance from a corresponding entry reference
        protected DescribedInstance(V reference) : base(reference)
        {
            // Duplicate attribute instances
            foreach (var attr in Reference.Attributes)
                Attributes.Add(attr.Duplicate());

            // Duplicate state instances
            foreach (var state in Reference.States)
                States.Add(state.Duplicate());

            // Duplicate qualifier instances
            foreach (var qualifier in Reference.Qualifiers)
                Qualifiers.Add(qualifier.Duplicate());
        }

        // Load a Described Instance from its exported JSON data
        protected DescribedInstance(JObject data, Registry<V> registry) : base(data, registry) 
        {
            J.ReadArray(data, VK.Attributes, obj => Attributes.Add(new Attribute(obj)));
            J.ReadArray(data, VK.States, obj => States.Add(new State(obj)));
            J.ReadArray(data, VK.Qualifiers, obj => Qualifiers.Add(new Qualifier(obj)));
        }

        public override void Export(JObject data)
        {
            base.Export(data);
            data[VK.Attributes] = J.WriteArray(Attributes);
            data[VK.States] = J.WriteArray(States);
            data[VK.Qualifiers] = J.WriteArray(Qualifiers);
        }

        public virtual void ImportExtraData(JObject data)
        {
            
        }

        // public override bool Equals(object obj)
        // {
        //     if (obj is DescribedInstance<V> other)
        //     {
                
        //     }

        //     return false;
        // }
    }
}