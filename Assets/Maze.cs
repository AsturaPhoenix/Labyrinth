using System;
using System.Collections.Generic;
using System.Linq;

public class Maze
{
    public static MutableVector<int> GetNeighbor(IList<int> coordinate, int side)
    {
        var neighbor = MutableVector<int>.CopyOf(coordinate);
        if (side < coordinate.Count)
        {
            --neighbor[side];
        }
        else
        {
            ++neighbor[side - coordinate.Count];
        }
        return neighbor;
    }

    public class Walls
    {
        private Maze maze;
        private ImmutableVector<int> coordinate;
        private bool contained;

        public Walls(Maze maze, ImmutableVector<int> coordinate)
        {
            this.maze = maze;
            this.coordinate = coordinate;
            contained = maze.cells.ContainsCoordinate(coordinate);
        }

        public bool this[int side]
        {
            get
            {
                var neighbor = GetNeighbor(coordinate, side);
                bool neighborContained = maze.cells.ContainsCoordinate(neighbor);

                if ((contained || !(coordinate == maze.Entrance || coordinate == maze.Exit)) ^ (neighborContained || !(neighbor == maze.Entrance || neighbor == maze.Exit)))
                    return false;

                if (side < coordinate.Dimensionality)
                    return contained ? maze.cells[coordinate][side] : neighborContained;
                else
                    return neighborContained ? maze.cells[neighbor][side - coordinate.Dimensionality] : contained;
            }

            set
            {
                var neighbor = GetNeighbor(coordinate, side);

                if (!(contained && maze.cells.ContainsCoordinate(neighbor))) throw new NotSupportedException("Not writable.");

                if (side < coordinate.Dimensionality)
                {
                    maze.cells[coordinate][side] = value;
                }
                else
                {
                    maze.cells[neighbor][side - coordinate.Dimensionality] = value;
                }
            }
        }
    }

    private readonly CartesianField<bool[]> cells;

    public ImmutableVector<int> Entrance, Exit;

    public Maze(params int[] dimensions)
    {
        cells = new CartesianField<bool[]>(new ImmutableVector<int>(dimensions), _ =>
        {
            var walls = new bool[dimensions.Length];
            for (int i = 0; i < walls.Length; ++i)
                walls[i] = true;
            return walls;
        });
    }

    public int Volume => cells.Volume;

    public ImmutableVector<int> Dimensions => cells.Dimensions;

    public Walls this[params int[] coordinate] => this[(IList<int>)coordinate];

    public Walls this[IList<int> coordinate] => new Walls(this, new ImmutableVector<int>(coordinate));

    public bool ContainsCoordinate(params int[] coordinate) => cells.ContainsCoordinate(coordinate);

    public bool ContainsCoordinate(IList<int> coordinate) => cells.ContainsCoordinate(coordinate);
}
