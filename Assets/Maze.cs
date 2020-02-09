using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private class Cell
    {
        public bool[] Walls;
        public Cell Subtree;
    }

    private struct Wall
    {
        public Cell owner;
        public int side;
        public Cell other;
    }

    private readonly int[] dimensions;
    private readonly Cell[] cells;

    public Maze(params int[] dimensions)
    {
        this.dimensions = (int[])dimensions.Clone();

        int[] magnitudes = new int[dimensions.Length + 1];
        magnitudes[0] = 1;
        for (int i = 0; i < dimensions.Length; ++i)
        {
            magnitudes[i + 1] = magnitudes[i] * dimensions[i];
        }

        int volume = magnitudes[dimensions.Length];
        cells = new Cell[volume];
        var walls = new List<Wall>(volume * dimensions.Length);
        for (int i = 0; i < volume; ++i)
        {
            cells[i] = new Cell { Walls = new bool[dimensions.Length] };
            for (int j = 0; j < dimensions.Length; ++j)
            {
                var cell = cells[i];
                cell.Walls[j] = true;
                cell.Subtree = cell;

                // inner walls only
                if (i % magnitudes[j + 1] >= magnitudes[j])
                {
                    walls.Add(new Wall
                    {
                        owner = cell,
                        side = j,
                        other = cells[i - magnitudes[j]]
                    });
                }
            }
        }

        // eliminate n - 1 walls
        for (int c = 1; c < volume; ++c)
        {
            int i = Random.Range(0, walls.Count);
            var wall = walls[i];
            walls.RemoveAt(i);

            if (ResolveSubtree(wall.owner) == ResolveSubtree(wall.other))
            {
                --c;
            }
            else
            {
                wall.owner.Walls[wall.side] = false;
                ResolveSubtree(wall.owner).Subtree = wall.other.Subtree;
            }
        }
    }

    private Cell ResolveSubtree(Cell cell)
    {
        while (cell.Subtree != cell)
        {
            cell.Subtree = cell.Subtree.Subtree;
            cell = cell.Subtree;
        }
        return cell;
    }

    public int GetDimension(int dimension) => dimensions[dimension];

    public bool[] GetWalls(params int[] coordinate)
    {
        int i = 0;
        // Outer edge cells are those that exceed bounds in exactly one dimension and are in bounds in others.
        int outerEdgeDimension = -1;
        for (int dim = dimensions.Length - 1; dim >= 0; --dim)
        {
            bool outerEdge = coordinate[dim] >= dimensions[dim];

            if (coordinate[dim] < 0 || outerEdge && outerEdgeDimension != -1)
                return new bool[dimensions.Length];

            if (outerEdge)
                outerEdgeDimension = dim;

            if (outerEdgeDimension == -1)
                i = i * dimensions[dim] + coordinate[dim];
        }

        if (outerEdgeDimension != -1)
        {
            bool[] walls = new bool[dimensions.Length];

            for (i = 0; i < dimensions.Length; ++i)
            {
                walls[i] = i == outerEdgeDimension;
            }
            return walls;
        }
        else
        {
            return (bool[])cells[i].Walls.Clone();
        }
    }
}
