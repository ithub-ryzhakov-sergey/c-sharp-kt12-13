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
        private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _dictionary;
        private readonly LinkedList<(TKey Key, TValue Value)> _linkedList;
        private readonly int _capacity;

        public int Capacity => _capacity;
        public int Count => _dictionary.Count;

        public LruCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0");

            _capacity = capacity;
            _dictionary = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity);
            _linkedList = new LinkedList<(TKey, TValue)>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null && typeof(TKey).IsClass)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var node))
            {
                // Move accessed node to the front (MRU position)
                _linkedList.Remove(node);
                _linkedList.AddFirst(node);
                value = node.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Set(TKey key, TValue value)
        {
            if (key == null && typeof(TKey).IsClass)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var existingNode))
            {
                // Update existing value and move to front
                _linkedList.Remove(existingNode);
                var newNode = _linkedList.AddFirst((key, value));
                _dictionary[key] = newNode;
            }
            else
            {
                // Add new item
                if (_dictionary.Count >= _capacity)
                {
                    // Remove LRU item
                    var lruNode = _linkedList.Last;
                    _dictionary.Remove(lruNode.Value.Key);
                    _linkedList.RemoveLast();
                }

                var newNode = _linkedList.AddFirst((key, value));
                _dictionary[key] = newNode;
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null && typeof(TKey).IsClass)
                throw new ArgumentNullException(nameof(key));

            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null && typeof(TKey).IsClass)
                throw new ArgumentNullException(nameof(key));

            if (_dictionary.TryGetValue(key, out var node))
            {
                _dictionary.Remove(key);
                _linkedList.Remove(node);
                return true;
            }

            return false;
        }
    }
}