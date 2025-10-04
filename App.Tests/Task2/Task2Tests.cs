using System;
using System.Collections.Generic;
using App.Task2;
using NUnit.Framework;

namespace App.Tests.Task2;

[TestFixture]
public class Task2Tests
{
    [Test]
    public void Some_Null_For_RefType_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null!));
    }

    [Test]
    public void None_HasValue_False_TryGet_And_GetValueOr()
    {
        var none = Option<int>.None();
        Assert.That(none.HasValue, Is.False);
        Assert.Throws<InvalidOperationException>(() => _ = none.Value);

        Assert.That(none.TryGet(out var v), Is.False);
        Assert.That(v, Is.EqualTo(default(int)));

        Assert.That(none.GetValueOrDefault(), Is.EqualTo(default(int)));
        Assert.That(none.GetValueOr(123), Is.EqualTo(123));
    }

    [Test]
    public void Some_HasValue_True_TryGet_And_GetValueOr()
    {
        var some = Option<int>.Some(5);
        Assert.That(some.HasValue, Is.True);
        Assert.That(some.Value, Is.EqualTo(5));

        Assert.That(some.TryGet(out var v), Is.True);
        Assert.That(v, Is.EqualTo(5));

        Assert.That(some.GetValueOrDefault(), Is.EqualTo(5));
        Assert.That(some.GetValueOr(123), Is.EqualTo(5));
    }

    [Test]
    public void Equality_Works_For_None_And_Some()
    {
        var a = Option<int>.None();
        var b = Option<int>.None();
        Assert.That(a.Equals(b), Is.True);
        Assert.That(a.Equals(Option<int>.Some(0)), Is.False);

        var s1 = Option<string>.Some("x");
        var s2 = Option<string>.Some("x");
        var s3 = Option<string>.Some("y");
        Assert.That(s1.Equals(s2), Is.True);
        Assert.That(s1.Equals(s3), Is.False);
    }

    [Test]

    public void GenericUtils_Swap_And_Copy()
    {
        var arr = new[] { 1, 2, 3 };
        GenericUtils.Swap(arr, 0, 2);
        Assert.That(arr, Is.EqualTo(new[] { 3, 2, 1 }));

        Assert.Throws<ArgumentOutOfRangeException>(() => GenericUtils.Swap(arr, -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => GenericUtils.Swap(arr, 0, 3));

        var copy = GenericUtils.Copy(arr);
        Assert.That(copy, Is.Not.SameAs(arr));
        Assert.That(copy, Is.EqualTo(new[] { 3, 2, 1 }));

        Assert.Throws<ArgumentNullException>(() => GenericUtils.Copy<int>(null!));
    }
}
