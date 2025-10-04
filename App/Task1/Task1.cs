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
        public int _capacity;
        public Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> _dict;
        public LinkedList<(TKey Key, TValue Value)> _list;

        public int Capacity => _capacity;
        public int Count => _dict.Count;

        public LruCache(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "капасити долежн быть больше 0");
            _capacity = capacity;
            _dict = new Dictionary<TKey, LinkedListNode<(TKey, TValue)>>(capacity);
            _list = new LinkedList<(TKey, TValue)>();
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (_dict.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _list.AddLast(node);
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

            if (_dict.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                var newNode = new LinkedListNode<(TKey, TValue)>((key, value));
                _list.AddLast(newNode);
                _dict[key] = newNode;
            }
            else
            {
                if (Count >= Capacity)
                {
                    var oldest = _list.First;
                    if (oldest != null)
                    {
                        _list.RemoveFirst();
                        _dict.Remove(oldest.Value.Key);
                    }
                }
                var newNode = new LinkedListNode<(TKey, TValue)>((key, value));
                _list.AddLast(newNode);
                _dict[key] = newNode;
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (_dict.TryGetValue(key, out var node))
            {
                _list.Remove(node);
                _dict.Remove(key);
                return true;
            }
            return false;
        }
    }
}