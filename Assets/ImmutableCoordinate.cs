using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct ImmutableCoordinate : IList<int>
{
    private int[] array;

    public ImmutableCoordinate(params int[] original)
    {
        array = (int[])original.Clone();
    }

    public ImmutableCoordinate(IList<int> original)
    {
        array = new int[original.Count];
        original.CopyTo(array, 0);
    }

    public MutableCoordinate ToMutable() => new MutableCoordinate((int[])array.Clone());

    public int this[int index] { get => array[index]; set => throw new NotSupportedException(); }

    public int Dimensionality => array.Length;

    int ICollection<int>.Count => Dimensionality;

    public bool IsReadOnly => true;

    public void Add(int item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(int item) => ((IList<int>)array).Contains(item);

    public void CopyTo(int[] array, int arrayIndex) => this.array.CopyTo(array, arrayIndex);

    public IEnumerator<int> GetEnumerator() => ((IList<int>)array).GetEnumerator();

    public int IndexOf(int item) => ((IList<int>)array).IndexOf(item);

    public void Insert(int index, int item) => throw new NotSupportedException();

    public bool Remove(int item) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

    public override bool Equals(Object other) => (other is MutableCoordinate || other is ImmutableCoordinate) && Enumerable.SequenceEqual(this, (IList<int>)other);

    public override int GetHashCode() => StructuralComparisons.StructuralEqualityComparer.GetHashCode(array);

    public static bool operator ==(ImmutableCoordinate a, object b) => Object.Equals(a, b);
    public static bool operator !=(ImmutableCoordinate a, object b) => !(a == b);
}
