
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
        private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _lruList;

        public int Capacity => _capacity;
        public int Count => _cache.Count;

        public LruCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0");

            _capacity = capacity;
            _cache = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
            _lruList = new LinkedList<CacheItem>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out var node))
            {
                // Перемещаем элемент в конец списка (самый недавно использованный)
                _lruList.Remove(node);
                _lruList.AddLast(node);

                value = node.Value.Value;
                return true;
            }

            value = default(TValue);
            return false;
        }

        public void Set(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out var existingNode))
            {
                // Обновляем существующий элемент
                existingNode.Value.Value = value;

                // Перемещаем в конец списка
                _lruList.Remove(existingNode);
                _lruList.AddLast(existingNode);
                return;
            }

            // Добавляем новый элемент
            if (_cache.Count >= _capacity)
            {
                // Удаляем самый старый элемент (первый в списке)
                var oldestNode = _lruList.First;
                _cache.Remove(oldestNode.Value.Key);
                _lruList.RemoveFirst();
            }

            var cacheItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(cacheItem);
            _cache[key] = newNode;
            _lruList.AddLast(newNode);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _cache.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_cache.TryGetValue(key, out var node))
            {
                _cache.Remove(key);
                _lruList.Remove(node);
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
