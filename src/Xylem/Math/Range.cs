using System;
using System.Collections.Generic;
using System.Text;

namespace Xylem.Vectors
{
    public struct IntRange
    {
        public static IntRange Arbitrary => new IntRange(int.MinValue / 2, int.MaxValue / 2);

        public static IntRange Opposites(int value) => new IntRange(-value, value);

        public readonly int Min;
        public readonly int Max;

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public IntRange WithMin(int min) => new IntRange(min, Max);

        public IntRange WithMax(int max) => new IntRange(Min, max);

        public int Clamp(int value)
        {
            if (value < Min)
                return Min;
            else if (Max < value)
                return Max;
            else 
                return value;
        }

        public bool Contains(int value) => Min <= value && value <= Max;

        public static IntRange operator -(IntRange range, int value)
        {
            return new IntRange(range.Min - value, range.Max - value);
        }

        public static IntRange operator +(IntRange range, int value)
        {
            return new IntRange(range.Min + value, range.Max + value);
        }
	}

    /**
     * This is a utility class for ensuring that values remain within certain bounds.
     * Bounds are both inclusive.
     */
    public class BoundedValue
    {
        public delegate int BoundProvider();

        public BoundProvider LowerBound { get; private set; }
        public BoundProvider UpperBound { get; private set; }

        protected int _value;
        public int Value
        {
            get => _value;
            set 
            {
                if (LowerBound() <= value && value <= UpperBound())
                    _value = value;
            }
        }

        public BoundedValue(BoundProvider lowerBound = null, BoundProvider upperBound = null, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            if (lowerBound == null)
                LowerBound = () => minValue;
            else LowerBound = lowerBound;

            if (upperBound == null)
                UpperBound = () => maxValue;
            else UpperBound = upperBound;
        }

        public static BoundedValue operator --(BoundedValue value)
        {
            value.Value--;
            return value;
        }

        public static BoundedValue operator ++(BoundedValue value)
        {
            value.Value++;
            return value;
        }

        public static BoundedValue operator +(BoundedValue value, int i)
        {
            value.Value += i;
            return value;
        }

        public static BoundedValue operator -(BoundedValue value, int i)
        {
            value.Value -= i;
            return value;
        }
    }
}
