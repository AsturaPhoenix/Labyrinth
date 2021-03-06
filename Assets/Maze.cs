﻿using System;
using System.Collections.Generic;

public interface IMaze
{
    ImmutableVector<int> Dimensions { get; }

    IMazeWalls this[params int[] coordinate] { get; }
    IMazeWalls this[IList<int> coordinate] { get; }

    ImmutableVector<int> Entrance { get; }
    ImmutableVector<int> Exit { get; }
}

public interface IMazeWalls
{
    bool this[int side] { get; set; }
}

public static class MazeExtensions
{
    public static IMaze Swizzle(this IMaze maze, params int[] swizzle) => Swizzle(maze, new ImmutableVector<int>(swizzle));
    public static IMaze Swizzle(this IMaze maze, ImmutableVector<int> swizzle) => new Maze.Swizzler(maze, swizzle);

    public static int IngressDirection(this IMaze maze, IList<int> coordinate)
    {
        if (coordinate.Count != maze.Dimensions.Dimensionality) throw new ArgumentException("Dimensionality mismatch.");

        for (int i = 0; i < maze.Dimensions.Dimensionality; ++i)
        {
            if (coordinate[i] < 0) return i + maze.Dimensions.Dimensionality;
            if (coordinate[i] >= maze.Dimensions[i]) return i;
        }

        throw new ArgumentException("Coordinate is not outside maze.");
    }
}

public class Maze : IMaze
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

    public class Walls : IMazeWalls
    {
        private readonly Maze maze;
        private readonly ImmutableVector<int> coordinate;
        private readonly bool contained;

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
    ImmutableVector<int> IMaze.Entrance => Entrance;
    ImmutableVector<int> IMaze.Exit => Exit;

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
    ImmutableVector<int> IMaze.Dimensions => Dimensions;

    public Walls this[params int[] coordinate] => this[(IList<int>)coordinate];
    public Walls this[IList<int> coordinate] => new Walls(this, new ImmutableVector<int>(coordinate));
    IMazeWalls IMaze.this[params int[] coordinate] => this[coordinate];
    IMazeWalls IMaze.this[IList<int> coordinate] => this[coordinate];

    public bool ContainsCoordinate(params int[] coordinate) => cells.ContainsCoordinate(coordinate);
    public bool ContainsCoordinate(IList<int> coordinate) => cells.ContainsCoordinate(coordinate);

    public bool Equals(Maze other) => other != null && Entrance == other.Entrance && Exit == other.Exit && cells == other.cells;
    public override bool Equals(object obj) => Equals(obj as Maze);
    public override int GetHashCode() => throw new NotImplementedException();
    public static bool operator ==(Maze a, Maze b) => Equals(a, b);
    public static bool operator !=(Maze a, Maze b) => !(a == b);

    public override string ToString()
    {
        switch (Dimensions.Dimensionality)
        {
            case 2:
                return MazeSerializer.BoxDrawing.Serialize2D(this);
            case 3:
                return MazeSerializer.BoxDrawing.Serialize3D(this);
            default:
                return Dimensions.ToString();
        }
    }

    public class Swizzler : IMaze
    {
        public class Walls : IMazeWalls
        {
            private readonly IMazeWalls backing;
            private readonly ImmutableVector<int> swizzle;

            public Walls(IMazeWalls backing, ImmutableVector<int> swizzle)
            {
                this.backing = backing;
                this.swizzle = swizzle;
            }

            private int Swizzle(int direction)
            {
                var swz = swizzle[direction % swizzle.Dimensionality];
                return direction < swizzle.Dimensionality ? swz : (swizzle.Dimensionality + swz) % (2 * swizzle.Dimensionality);
            }

            public bool this[int side]
            {
                get => backing[Swizzle(side)];
                set => backing[Swizzle(side)] = value;
            }
        }

        private readonly IMaze maze;
        private readonly ImmutableVector<int> swizzle;

        /// <param name="swizzle">Swizzle is specified like a direction. Elements less than the
        /// dimensionality map the specified incoming dimension to the outgoing dimension at that
        /// position. Otherwise the dimension is flipped.</param>
        public Swizzler(IMaze maze, ImmutableVector<int> swizzle)
        {
            this.maze = maze;
            this.swizzle = swizzle;
        }

        private int[] Swizzle(IList<int> coordinate)
        {
            var swizzled = new int[coordinate.Count];
            for (int i = 0; i < coordinate.Count; ++i)
            {
                int sourceDim = swizzle[i];
                if (sourceDim < swizzle.Dimensionality)
                {
                    swizzled[i] = coordinate[sourceDim];
                }
                else
                {
                    sourceDim -= swizzle.Dimensionality;
                    swizzled[i] = maze.Dimensions[sourceDim] - 1 - coordinate[sourceDim];
                }
            }
            return swizzled;
        }

        public ImmutableVector<int> Dimensions => maze.Dimensions;

        public Walls this[params int[] coordinate] => this[(IList<int>)coordinate];
        public Walls this[IList<int> coordinate] => new Walls(maze[Swizzle(coordinate)], swizzle);
        IMazeWalls IMaze.this[params int[] coordinate] => this[coordinate];
        IMazeWalls IMaze.this[IList<int> coordinate] => this[coordinate];

        public ImmutableVector<int> Entrance => new ImmutableVector<int>(Swizzle(maze.Entrance));
        public ImmutableVector<int> Exit => new ImmutableVector<int>(Swizzle(maze.Exit));
    }
}
