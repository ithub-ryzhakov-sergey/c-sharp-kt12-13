
using System;
using System.Collections.Generic;

namespace App.Task1
{
    public interface ICache<TKey, TValue>
    {
        int Capacity { get; }
        int Count { get; }
        bool TryGet(TKey key, out TValue value);
        void Set(TKey key, TValue value);
        bool ContainsKey(TKey key);
        bool Remove(TKey key);
    }

    public class LruCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, (TValue Value, int Index)> _dictionary;
        private readonly List<TKey> _accessOrder;

        public int Capacity => _capacity;
        public int Count => _dictionary.Count;

        public LruCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _capacity = capacity;
            _dictionary = new Dictionary<TKey, (TValue, int)>();
            _accessOrder = new List<TKey>(capacity);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var item))
            {
                UpdateAccessOrder(key, item.Index);
                value = item.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Set(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var existingItem))
            {
                UpdateAccessOrder(key, existingItem.Index);
                _dictionary[key] = (value, _accessOrder.Count - 1);
            }
            else
            {
                if (Count >= Capacity)
                {
                    var lruKey = _accessOrder[0];
                    _dictionary.Remove(lruKey);
                    _accessOrder.RemoveAt(0);
                }

                _accessOrder.Add(key);
                _dictionary[key] = (value, _accessOrder.Count - 1);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var item))
            {
                _accessOrder.RemoveAt(item.Index);
                _dictionary.Remove(key);

                for (int i = item.Index; i < _accessOrder.Count; i++)
                {
                    var currentKey = _accessOrder[i];
                    _dictionary[currentKey] = (_dictionary[currentKey].Value, i);
                }

                return true;
            }

            return false;
        }

        private void UpdateAccessOrder(TKey key, int currentIndex)
        {
            _accessOrder.RemoveAt(currentIndex);
            _accessOrder.Add(key);

            for (int i = currentIndex; i < _accessOrder.Count; i++)
            {
                var currentKey = _accessOrder[i];
                _dictionary[currentKey] = (_dictionary[currentKey].Value, i);
            }
        }
    }
}