using System;
using System.Collections.Generic;

public static class LongestPathEndpointGenerator
{
    private struct PathMetric : IComparable<PathMetric>
    {
        public int Forks, Distance;

        public static PathMetric operator +(PathMetric a, PathMetric b)
        {
            return new PathMetric { Forks = a.Forks + b.Forks, Distance = a.Distance + b.Distance };
        }

        public int CompareTo(PathMetric other)
        {
            int cmp = Forks.CompareTo(other.Forks);
            if (cmp == 0)
                cmp = Distance.CompareTo(other.Distance);
            return cmp;
        }

        public override bool Equals(object other) => other is PathMetric && CompareTo((PathMetric)other) == 0;

        public override int GetHashCode()
        {
            var hashCode = -291104344;
            hashCode = hashCode * -1521134295 + Forks.GetHashCode();
            hashCode = hashCode * -1521134295 + Distance.GetHashCode();
            return hashCode;
        }

        public static bool operator >(PathMetric a, PathMetric b) => a.CompareTo(b) > 0;
        public static bool operator <(PathMetric a, PathMetric b) => a.CompareTo(b) < 0;
        public static bool operator >=(PathMetric a, PathMetric b) => a.CompareTo(b) >= 0;
        public static bool operator <=(PathMetric a, PathMetric b) => a.CompareTo(b) <= 0;
        public static bool operator ==(PathMetric a, PathMetric b) => a.CompareTo(b) == 0;
        public static bool operator !=(PathMetric a, PathMetric b) => a.CompareTo(b) != 0;
    }

    private class Path
    {
        public IList<int> Coordinate;
        public PathMetric Metric;
        public bool DeadEnd;
    }

    private static int ComparePaths(PathMetric commonA, Path pathA, PathMetric commonB, Path pathB)
    {
        if (pathA.DeadEnd && !pathB.DeadEnd) return 1;
        if (!pathA.DeadEnd && pathB.DeadEnd) return -1;
        return (commonA + pathA.Metric).CompareTo(commonB + pathB.Metric);
    }

    private class SearchResult
    {
        public PathMetric CommonMetric;
        public Path Best, Second;

        public void Merge(SearchResult other)
        {
            if (other.Best == null) return;

            if (Best == null)
            {
                CommonMetric = other.CommonMetric;
                Best = other.Best;
                Second = other.Second;
            }
            else if (ComparePaths(other.CommonMetric, other.Best, CommonMetric, Best) > 0)
            {
                if (other.Second != null && ComparePaths(new PathMetric(), other.Second, other.CommonMetric + CommonMetric, Best) > 0)
                {
                    CommonMetric = other.CommonMetric;
                    Best = other.Best;
                    Second = other.Second;
                }
                else
                {
                    Best.Metric += CommonMetric;
                    other.Best.Metric += other.CommonMetric;
                    CommonMetric = new PathMetric();

                    Second = Best;
                    Best = other.Best;
                }
            }
            else if (Second == null || ComparePaths(CommonMetric + other.CommonMetric, other.Best, new PathMetric(), Second) > 0)
            {
                Best.Metric += CommonMetric;
                other.Best.Metric += other.CommonMetric;
                CommonMetric = new PathMetric();

                Second = other.Best;
            }
        }
    }

    public static void Generate(Maze maze, params int[] seed)
    {
        if (!maze.ContainsCoordinate(seed)) throw new ArgumentOutOfRangeException("seed", seed, "Seed must be within maze.");

        var result = Search(maze, seed, -1);

        IList<int> exit = result.Best.Coordinate, entrance = (result.Second == null ? result.Best : result.Second).Coordinate;
        List<int> entranceDirections = EdgeDirections(maze, entrance), exitDirections = EdgeDirections(maze, exit);
        int entranceDirection, exitDirection;
        PickDirections(entranceDirections, exitDirections, out entranceDirection, out exitDirection);
        entrance = Maze.GetNeighbor(entrance, entranceDirection);
        exit = Maze.GetNeighbor(exit, exitDirection);

        maze.Entrance = new ImmutableVector<int>(entrance);
        maze.Exit = new ImmutableVector<int>(exit);
    }

    private static SearchResult Search(Maze maze, IList<int> coordinate, int incomingDirection)
    {
        int dims = maze.Dimensions.Dimensionality;
        Maze.Walls cell = maze[coordinate];
        var result = new SearchResult();
        int openings = 0;

        for (int i = 0; i < 2 * maze.Dimensions.Dimensionality; ++i)
        {
            if (!cell[i])
            {
                ++openings;

                if (i != incomingDirection)
                    result.Merge(Search(maze, Maze.GetNeighbor(coordinate, i), i < dims ? dims + i : i - dims));
            }
        }

        if (result.Second == null && IsEdge(maze, coordinate))
                result.Merge(new SearchResult { Best = new Path { Coordinate = coordinate, DeadEnd = openings <= 1 } });

        if (openings > 2)
            result.CommonMetric.Forks += openings - 1;
        ++result.CommonMetric.Distance;

        return result;
    }

    // Edges are cells where at least one dimension is at an extreme.
    private static bool IsEdge(Maze maze, IList<int> coordinate)
    {
        for (int i = 0; i < coordinate.Count; ++i)
        {
            if (coordinate[i] == 0 || coordinate[i] == maze.Dimensions[i] - 1)
                return true;
        }
        return false;
    }

    private static List<int> EdgeDirections(Maze maze, IList<int> coordinate)
    {
        var candidates = new List<int>(coordinate.Count);
        for (int i = 0; i < coordinate.Count; ++i)
        {
            if (coordinate[i] == 0)
                candidates.Add(i);
            if (coordinate[i] == maze.Dimensions[i] - 1)
                candidates.Add(coordinate.Count + i);
        }

        return candidates;
    }

    private static void PickDirections(List<int> entranceCandidates, List<int> exitCandidates, out int entranceDirection, out int exitDirection)
    {
        entranceDirection = entranceCandidates[UnityEngine.Random.Range(0, entranceCandidates.Count)];
        exitDirection = exitCandidates[UnityEngine.Random.Range(0, exitCandidates.Count)];
    }
}
