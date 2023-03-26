
using System;
using System.Collections.Generic;

namespace Xylem.Functional
{
    public class Property<V>
    {
        private readonly bool _mutable;
        private V _value;

        public V Value
        {
            get => _value;
            set 
            {
                if (_mutable)
                {
                    foreach (var observer in _observers)
                        observer(_value, value);
                    _value = value;
                }
            }
        }

        public delegate void Observer(V previous, V next);
        private readonly List<Observer> _observers = new List<Observer>();

        public Property(V value, bool mutable = true)
        {
            _value = value;
            _mutable = mutable;
        }

        public void AddObserver(Observer observer) => _observers.Add(observer);

        [Obsolete]
        public static Property<V> operator +(Property<V> attribute, Observer observer)
        {
            attribute._observers.Add(observer);
            return attribute;;
        }
    }
}