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
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _dictionary;
        private readonly LinkedList<CacheItem> _linkedList;

        public int Capacity => _capacity;
        public int Count => _dictionary.Count;

        public LruCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _capacity = capacity;
            _dictionary = new Dictionary<TKey, LinkedListNode<CacheItem>>();
            _linkedList = new LinkedList<CacheItem>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_dictionary.TryGetValue(key, out var node))
            {
                _linkedList.Remove(node);
                _linkedList.AddLast(node);

                value = node.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Set(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_dictionary.TryGetValue(key, out var existingNode))
            {
                existingNode.Value.Value = value;
                _linkedList.Remove(existingNode);
                _linkedList.AddLast(existingNode);
            }
            else
            {
                if (_dictionary.Count >= _capacity)
                {
                    var oldestNode = _linkedList.First;
                    _dictionary.Remove(oldestNode.Value.Key);
                    _linkedList.RemoveFirst();
                }

                var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
                _dictionary[key] = newNode;
                _linkedList.AddLast(newNode);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _dictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_dictionary.TryGetValue(key, out var node))
            {
                _dictionary.Remove(key);
                _linkedList.Remove(node);
                return true;
            }

            return false;
        }

        private class CacheItem
        {
            public TKey Key { get; }
            public TValue Value { get; set; }

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}