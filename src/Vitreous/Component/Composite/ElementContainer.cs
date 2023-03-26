
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xylem.Reference;
using Xylem.Reflection;

namespace Vitreous.Component.Composite
{
    public abstract class ElementContainer : JsonComposite, IEnumerable<Element>
    {
        // Data is required so ElementContainer extensions can decide what extra
        // information they need (uniform construction without necessarily uniform data).
        public static V Create<V>(System.Type containerType, JObject data) where V : ElementContainer
        {
            return Introspector.Instantiate<V>(containerType, typeof(ElementContainer), data);
        }

        protected ElementContainer(JObject data)
        {
        }

        public abstract IEnumerator<Element> GetEnumerator();

        public abstract void Add(Element element);
        public abstract void Remove(Element element);

        public abstract int Size();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class ListContainer : ElementContainer
    {
        protected readonly List<Element> Elements = new List<Element>();

        protected ListContainer(JObject data) : base(data)
        {
        }

        public override void Add(Element element) => Elements.Add(element);
        public override void Remove(Element element) => Elements.Remove(element);

        public override int Size() => Elements.Count;

        public override IEnumerator<Element> GetEnumerator() => Elements.GetEnumerator();
    }

    public sealed class NullContainer : ListContainer
    {
        public NullContainer(JObject data) : base(data)
        {

        }

        public sealed override void Add(Element element) {}
        public sealed override void Remove(Element element) {}

        public override void Export(JObject data)
        {
            
        }
    }

    // A simple heap of elements.
    public class HeapContainer : ListContainer
    {
        public HeapContainer(JObject data) : base(data)
        {

        }

        public override void Export(JObject data)
        {
            throw new System.NotImplementedException();
        }
    }
}