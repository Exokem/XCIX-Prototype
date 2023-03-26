
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Xylem.Functional
{
    public interface IIdentifiable
    {
        string Identifier { get; }
    }

    public abstract class IndexedMap<K, V> : KeyedCollection<K, V>
    {
        public V Get(int index) => Items[index];
    }

    public class LinkedHashSet<V> : IndexedMap<V, V>
    {
        protected override V GetKeyForItem(V item)
        {
            return item;
        }
    }

    public class OrderedSet<V> : IndexedMap<string, V> where V : IIdentifiable
    {
        protected override string GetKeyForItem(V item)
        {
            return item.Identifier;
        }
    }

    

    public static class CollectionExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            else return defaultValue;
        }

        public static void AddAll<V>(this ICollection<V> collection, params V[] values)
        {
            foreach (V value in values)
                collection.Add(value);
        }

        public static void AddIf<TValue>(this ISet<TValue> set, bool condition, TValue value)
        {
            if (condition)
                set.Add(value);
        }
    }
}