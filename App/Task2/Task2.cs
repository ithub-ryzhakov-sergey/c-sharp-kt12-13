using System;
using System.Collections.Generic;

namespace App.Task2
{
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        public bool HasValue => _hasValue;

        public T Value
        {
            get
            {
                if (!_hasValue)
                    throw new InvalidOperationException("Option does not have a value");
                return _value;
            }
        }

        private Option(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public static Option<T> None()
        {
            return new Option<T>(default(T), false);
        }

        public static Option<T> Some(T value)
        {
            if (value == null && default(T) == null)
                throw new ArgumentNullException(nameof(value));

            return new Option<T>(value, true);
        }

        public bool TryGet(out T value)
        {
            value = _hasValue ? _value : default(T);
            return _hasValue;
        }

        public T GetValueOrDefault()
        {
            return _hasValue ? _value : default(T);
        }

        public T GetValueOr(T fallback)
        {
            return _hasValue ? _value : fallback;
        }

        public bool Equals(Option<T> other)
        {
            if (!_hasValue && !other._hasValue)
                return true;

            if (_hasValue && other._hasValue)
                return EqualityComparer<T>.Default.Equals(_value, other._value);

            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is Option<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            if (!_hasValue)
                return 0;

            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _hasValue ? $"Some({_value})" : "None";
        }
    }

    public static class GenericUtils
    {
        public static T Max<T>(IEnumerable<T> items) where T : IComparable<T>
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            using var enumerator = items.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new ArgumentException("Sequence contains no elements", nameof(items));

            T max = enumerator.Current;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.CompareTo(max) > 0)
                {
                    max = enumerator.Current;
                }
            }

            return max;
        }

        public static void Swap<T>(T[] array, int i, int j)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (i < 0 || i >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(i), "Index is out of range");

            if (j < 0 || j >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(j), "Index is out of range");

            if (i == j)
                return;

            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public static T[] Copy<T>(T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            T[] copy = new T[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                copy[i] = items[i];
            }
            return copy;
        }
    }
}
