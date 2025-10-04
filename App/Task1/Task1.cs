namespace App.Task1;

public interface ICache<TKey, TValue>
{
    int Capacity { get; }
    int Count { get; }
    public bool TryGet(TKey key, out TValue value);
    public void Set(TKey key, TValue value);
    public bool ContainsKey(TKey key);
    public bool Remove(TKey key);

}
public class LruCache<TKey, TValue> : ICache<TKey, TValue>
{
    public TKey key { get; set; }
    int Capacity { get; }
    int Count { get; }
    Dictionary<TKey, TValue> keyValuePairs { get; set; }
    LruCache(int capacity)
    {
        if (capacity < 0) { throw new ArgumentOutOfRangeException(); }
        else
        {
            
        }
    }
    public bool TryGet(TKey key, out TValue value)
    {
        return keyValuePairs.ContainsKey(key) && keyValuePairs.ContainsValue(value);
    }
}
