using System;
using System.Collections.Generic;
using App.Task1;
using NUnit.Framework;

namespace App.Tests.Task1;

[TestFixture]
public class Task1Tests
{
    [Test]
    public void Constructor_InvalidCapacity_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<string, int>(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new LruCache<string, int>(-5));
    }

    [Test]
    public void Set_And_TryGet_Basic()
    {
        var cache = new LruCache<string, int>(3);
        cache.Set("a", 1);
        cache.Set("b", 2);
        cache.Set("c", 3);

        Assert.That(cache.Count, Is.EqualTo(3));

        Assert.That(cache.TryGet("a", out var va), Is.True);
        Assert.That(va, Is.EqualTo(1));
        Assert.That(cache.TryGet("b", out var vb), Is.True);
        Assert.That(vb, Is.EqualTo(2));
        Assert.That(cache.TryGet("c", out var vc), Is.True);
        Assert.That(vc, Is.EqualTo(3));
    }

    [Test]
    public void Set_OverCapacity_Evicts_Lru()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1); // LRU: a
        cache.Set("b", 2); // LRU: a, MRU: b
        // Access a to make it MRU
        Assert.That(cache.TryGet("a", out _), Is.True); // LRU: b, MRU: a

        cache.Set("c", 3); // should evict b

        Assert.That(cache.ContainsKey("b"), Is.False);
        Assert.That(cache.ContainsKey("a"), Is.True);
        Assert.That(cache.ContainsKey("c"), Is.True);
        Assert.That(cache.Count, Is.EqualTo(2));
    }

    [Test]
    public void Update_ExistingKey_Moves_To_Mru()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1); // LRU: a
        cache.Set("b", 2); // LRU: a, MRU: b
        cache.Set("a", 11); // a becomes MRU, order: b (LRU), a (MRU)

        cache.Set("c", 3); // should evict b

        Assert.That(cache.ContainsKey("b"), Is.False);
        Assert.That(cache.TryGet("a", out var va), Is.True);
        Assert.That(va, Is.EqualTo(11));
        Assert.That(cache.ContainsKey("c"), Is.True);
    }

    [Test]
    public void Remove_Works_And_Returns_Flag()
    {
        var cache = new LruCache<string, int>(2);
        cache.Set("a", 1);
        cache.Set("b", 2);

        Assert.That(cache.Remove("a"), Is.True);
        Assert.That(cache.Remove("a"), Is.False);
        Assert.That(cache.Count, Is.EqualTo(1));
    }

    [Test]
    public void Null_Key_Throws_For_Reference_TKey()
    {
        var cache = new LruCache<string, int>(2);
        Assert.Throws<ArgumentNullException>(() => cache.Set(null!, 42));
        Assert.Throws<ArgumentNullException>(() => cache.ContainsKey(null!));
        Assert.Throws<ArgumentNullException>(() => cache.Remove(null!));
        Assert.Throws<ArgumentNullException>(() => cache.TryGet(null!, out _));
    }

    [Test]
    public void Interface_Contract_Works()
    {
        ICache<string, int> cache = new LruCache<string, int>(2);
        cache.Set("x", 7);
        Assert.That(cache.TryGet("x", out var v), Is.True);
        Assert.That(v, Is.EqualTo(7));
    }
}
