using System;
using System.Collections;
using System.Collections.Generic;

public class CartesianField<T> : IEnumerable<T>
{
    public readonly ImmutableVector<int> Dimensions;

    private T[] cells;

    public CartesianField(ImmutableVector<int> dimensions, Func<ImmutableVector<int>, T> factory)
    {
        Dimensions = dimensions;

        int volume;
        if (dimensions.Dimensionality == 0)
        {
            volume = 0;
        }
        else
        {
            volume = 1;
            foreach (int dimension in dimensions)
                volume *= dimension;
        }

        cells = new T[volume];
        foreach (var coordinate in Coordinates)
            this[coordinate] = factory(coordinate);
    }

    public int Volume => cells.Length;

    public T this[params int[] coordinate]
    {
        get => this[(IList<int>)coordinate];
        set => this[(IList<int>)coordinate] = value;
    }

    private int Flatten(IList<int> coordinate)
    {
        int i = 0;
        for (int di = 0; di < coordinate.Count; ++di)
        {
            i = i * Dimensions[di] + coordinate[di];
        }
        return i;
    }

    public T this[IList<int> coordinate]
    {
        get => cells[Flatten(coordinate)];
        private set => cells[Flatten(coordinate)] = value;
    }

    public IEnumerable<ImmutableVector<int>> Coordinates
    {
        get
        {
            if (Volume == 0) yield break;

            var current = MutableVector<int>.Zero(Dimensions.Dimensionality);
            yield return current.ToImmutable();

            for (int i = 0; i < current.Dimensionality;)
            {
                if (++current[i] < Dimensions[i])
                {
                    i = 0;
                    yield return current.ToImmutable();
                }
                else
                {
                    current[i++] = 0;
                }
            }
        }
    }

    public bool ContainsCoordinate(params int[] coordinate) => ContainsCoordinate((IList<int>)coordinate);

    public bool ContainsCoordinate(IList<int> coordinate)
    {
        for (int i = 0; i < Dimensions.Dimensionality; ++i)
        {
            if (coordinate[i] < 0 || coordinate[i] >= Dimensions[i])
                return false;
        }
        return true;
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)cells).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => cells.GetEnumerator();
}
