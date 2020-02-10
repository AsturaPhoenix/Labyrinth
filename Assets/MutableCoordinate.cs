using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MutableCoordinate : IList<int>
{
    private int[] array;

    public MutableCoordinate(params int[] backing)
    {
        array = backing;
    }

    public static MutableCoordinate Zero(int dimensionality) => new MutableCoordinate(new int[dimensionality]);
    public static MutableCoordinate CopyOf(IList<int> original)
    {
        int[] array = new int[original.Count];
        original.CopyTo(array, 0);
        return new MutableCoordinate(array);
    }

    public ImmutableCoordinate ToImmutable() => new ImmutableCoordinate(array);

    public int this[int index] { get => array[index]; set => array[index] = value; }

    public int Dimensionality => array.Length;

    int ICollection<int>.Count => Dimensionality;

    public bool IsReadOnly => false;

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

    public static bool operator ==(MutableCoordinate a, object b) => Object.Equals(a, b);
    public static bool operator !=(MutableCoordinate a, object b) => !(a == b);
}
