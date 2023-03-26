
using Newtonsoft.Json.Linq;
using Xylem.Reference;
using Xylem.Registration;
using Xylem.Component;
using Vitreous.Registration;

namespace Vitreous.Component.Core
{
    public class QualifierEntry : RegistryEntry
    {
        public readonly bool BaseValue;

        public QualifierEntry(JObject data) : base(data)
        {
            BaseValue = J.ReadBool(data, K.BaseValue, false);
        }
    }

    public class Qualifier : AbstractInstance<QualifierEntry>, IDuplicable<Qualifier>
    {
        public bool Value { get; set; }

        public Qualifier(QualifierEntry qualifier, bool value) : base(qualifier)
        {
            Value = value;
        }

        public Qualifier(JObject data) : base(data, Registries.Qualifiers)
        {
            Value = J.ReadBool(data, K.Value, Reference.BaseValue);
        }

        // public void ImportExtraData(JObject data)
        // {

        // }

        public override void Export(JObject data)
        {
            base.Export(data);
            data[K.Value] = Value;
        }

        public void ImportExtraData(JObject data)
        {
            Value = J.ReadBool(data, K.Value, Value);
        }

        public Qualifier Duplicate()
        {
            return new Qualifier(Reference, Value);
        }
    }
}