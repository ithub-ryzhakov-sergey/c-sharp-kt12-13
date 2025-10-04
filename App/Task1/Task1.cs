namespace App.Task1;

public interface ICache<TKey, TValue>
{
    int Capacity { get; }
    int Count { get; }
    public bool TryGet(TKey key, out TValue value);
    public void Set(TKey key, TValue value);
    public bool ContainsKey(TKey key);
    public bool Remove(TKey key);
};

public class LruCache<TKey, TValue> : ICache<TKey, TValue>
{
    private Dictionary<TKey, TValue> keyValuePairs = new Dictionary<TKey, TValue>();
    private Dictionary<TKey, int> keyUsages = new Dictionary<TKey, int>();

    public int Capacity { get; private set; }

    public int Count => keyValuePairs.Count;

    public bool ContainsKey(TKey key)
    {
        return keyValuePairs.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        if (ContainsKey(key))
        {
            keyValuePairs.Remove(key);
            keyUsages.Remove(key);
            return true;
        }

        return false;
    }

    public void Set(TKey key, TValue value)
    {
        if (key == null) { throw new ArgumentNullException(); }
        if (keyValuePairs.ContainsKey(key))
        {
            keyValuePairs[key] = value;
            keyUsages[key]++;
        }
        else
        {
            if (Count >= Capacity)
            {
                var keyWithMinUsages = keyUsages
                    .MinBy(x => x.Value)
                    .Key;
                Remove(keyWithMinUsages);
            }

            keyValuePairs[key] = value;
            keyUsages[key] = 1;
        }
    }

    public bool TryGet(TKey key, out TValue value)
    {
        if (keyValuePairs.TryGetValue(key, out value))
        {
            keyUsages[key]++;
            return true;
        }
        return false;
    }

    public LruCache(int capacity)
    {
        if (capacity <= 0) { throw new ArgumentOutOfRangeException(); }
        Capacity = capacity;
    }
}