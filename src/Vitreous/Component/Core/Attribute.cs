
using Newtonsoft.Json.Linq;
using Xylem.Functional;
using Xylem.Reference;
using Xylem.Registration;
using Xylem.Component;
using Xylem.Vectors;
using Vitreous.Registration;
using Vitreous.Reference;

namespace Vitreous.Component.Core
{
    /**
     * Attribute definitions are represented by instances of this Attribute class.\n
     *
     * This includes information that is absolute to the attribute itself, like the
     * identifier or description, for example.
     */
    public class AttributeEntry : RegistryEntry
    {
        public AttributeEntry(JObject data) : base(data)
        {

        }
    }

    /**
     * Modifiers exist exclusively to modify attribute values. Accordingly, they are
     * only ever linked to an AttributeInstance since that is the only version of an
     * attribute that may hold a specific value.\n
     *
     * Modifiers have a value that may change internally based on a defined time frame
     * or state.
     */
    public class Modifier : NamedComponent
    {
        public int Value { get; private set; }

        public Modifier(JObject data) : base(data)
        {
            Value = J.ReadInt(data, K.Value, 0);
        }

        public override void Export(JObject data)
        {
            base.Export(data);
            data[K.Value] = Value;
        }
    }

    /**
     * Attribute implementations are represented by instances of this AttributeInstance
     * class.\n
     *
     * This includes information that can vary between implementations of an attribute.
     * For example, many different objects may possess the same strength attribute, but
     * will likely have different values for it.\n
     *
     * Another important distinction between this class and the Attribute class is that
     * AttributeInstances are not registered. They exist only within the context of some
     * other element of the program.\n
     */
    public class Attribute : AbstractInstance<AttributeEntry>, IDuplicable<Attribute>
    {
        public readonly int BaseValue;
        public readonly IntRange ValueRange;

        public int Value { get; set; }

        public readonly OrderedSet<Modifier> Modifiers = new OrderedSet<Modifier>();
        // private readonly EntryMap<Modifier> _modifiers = new EntryMap<Modifier>();

        public Attribute(AttributeEntry attribute, int baseValue, int min, int max) : base(attribute)
        {
            BaseValue = baseValue;
            ValueRange = new IntRange(min, max);
        }

        public Attribute(JObject data) : base(data, Registries.Attributes)
        {
            int min = J.ReadInt(data, K.MinValue, int.MinValue);
            int max = J.ReadInt(data, K.MaxValue, int.MaxValue);

            BaseValue = J.ReadInt(data, K.BaseValue, max);
            Value = J.ReadInt(data, K.Value, BaseValue);

            ValueRange = new IntRange(min, max);

            J.ReadArray(data, VK.Modifiers, obj => Modifiers.Add(new Modifier(obj)));
        }

        public void ImportExtraData(JObject data)
        {
            Value = J.ReadInt(data, K.Value, Value);

            J.ReadArray(data, VK.Modifiers, obj => Modifiers.Add(new Modifier(obj)));
        }

        // public int Value => BaseValue + _modifiers.Sum(modifier => modifier.Value);

        // public void SetMinValue(int min)
        // {
        //     ValueRange = ValueRange.WithMin(min);
        // }

        // public void SetMaxValue(int max)
        // {
        //     ValueRange = ValueRange.WithMax(max);
        // }

        public Attribute Duplicate()
        {
            return new Attribute(Reference, BaseValue, ValueRange.Min, ValueRange.Max);
        }

        // Used for saving data that will not be recovered from the base attribute 
        // Base values and min/max values, e.g., should be constant and thus defined
        // by the attribute instance itself, not saved here
        public override void Export(JObject data)
        {
            base.Export(data);
            // data[K.BaseValue] = BaseValue;
            data[K.Value] = Value;
            // data[K.MinValue] = ValueRange.Min;
            // data[K.MaxValue] = ValueRange.Max;
            data[VK.Modifiers] = J.WriteArray(Modifiers);
        }
    }
}