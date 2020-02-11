using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ImmutableVector<T> : IList<T>
{
    private readonly T[] array;

    public ImmutableVector(params T[] original)
    {
        array = (T[])original.Clone();
    }

    public ImmutableVector(IList<T> original)
    {
        array = new T[original.Count];
        original.CopyTo(array, 0);
    }

    public MutableVector<T> ToMutable() => new MutableVector<T>((T[])array.Clone());

    public T this[int index] { get => array[index]; set => throw new NotSupportedException(); }

    public int Dimensionality => array.Length;

    int ICollection<T>.Count => Dimensionality;

    public bool IsReadOnly => true;

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => ((IList<T>)array).Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => this.array.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => ((IList<T>)array).GetEnumerator();

    public int IndexOf(T item) => ((IList<T>)array).IndexOf(item);

    public void Insert(int index, T item) => throw new NotSupportedException();

    public bool Remove(T item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

    public override bool Equals(Object other) => (other is MutableVector<T> || other is ImmutableVector<T>) && Enumerable.SequenceEqual(this, (IList<T>)other);

    public override int GetHashCode() => StructuralComparisons.StructuralEqualityComparer.GetHashCode(array);

    public static bool operator ==(ImmutableVector<T> a, object b) => Object.Equals(a, b);
    public static bool operator !=(ImmutableVector<T> a, object b) => !(a == b);
}
