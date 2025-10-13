namespace App.Task1;

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
    public int Capacity { get; private set; }
    public int Count { get => this._content.Count; }
    public LruCache(int capacity)
    {
        if (!(capacity > 0))
        {
            throw new ArgumentOutOfRangeException();
        }
        this.Capacity = capacity;
        this._content = new Dictionary<TKey, TValue>();
        this._priority = new LinkedList<TKey>();
    }
    public bool ContainsKey(TKey key)
    {
        if (key == null) { throw new ArgumentNullException(); }
        return _content.ContainsKey(key);
    }
    public bool Remove(TKey key)
    {
        if (key == null) { throw new ArgumentNullException(); }
        if (_content.ContainsKey(key))
        {
            _content.Remove(key);
            _priority.Remove(key);
            return true;
        }
        return false;
    }

    public void Set(TKey key, TValue value)
    {
        if (key == null) { throw new ArgumentNullException(); }
        if (_priority.Count >= this.Capacity)
        {
            _content.Remove(_priority.Last.Value);
            _priority.RemoveLast();
        }
        if (_content.ContainsKey(key))
        {
            _content[key] = value;
            _priority.Remove(key);
        }
        else
        {
            _content.Add(key, value);
        }
        _priority.AddFirst(key);
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (key == null) { throw new ArgumentNullException(); }
        bool res = _content.TryGetValue(key, out value);
        if (res)
        {
            _priority.Remove(key);
            _priority.AddFirst(key);
        }
        return res;
    }
    private Dictionary<TKey, TValue> _content;
    private LinkedList<TKey> _priority;
}