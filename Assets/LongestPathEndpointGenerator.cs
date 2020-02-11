using System;
using System.Collections.Generic;

public static class LongestPathEndpointGenerator
{
    private class Path
    {
        public int Distance;
        public IList<int> Coordinate;
    }

    private class SearchResult
    {
        public int CommonDistance;
        public Path Best, Second;

        public void Merge(SearchResult other)
        {
            if (other.Best == null) return;

            if (Best == null)
            {
                CommonDistance = other.CommonDistance;
                Best = other.Best;
                Second = other.Second;
            }
            else if (other.CommonDistance + other.Best.Distance > CommonDistance + Best.Distance)
            {
                if (other.Second != null && other.Second.Distance > other.CommonDistance + CommonDistance + Best.Distance)
                {
                    CommonDistance = other.CommonDistance;
                    Best = other.Best;
                    Second = other.Second;
                }
                else
                {
                    Best.Distance += CommonDistance;
                    other.Best.Distance += other.CommonDistance;
                    CommonDistance = 0;

                    Second = Best;
                    Best = other.Best;
                }
            }
            else if (Second == null || CommonDistance + other.CommonDistance + other.Best.Distance > Second.Distance)
            {
                Best.Distance += CommonDistance;
                other.Best.Distance += other.CommonDistance;
                CommonDistance = 0;

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

        if (openings <= 1 && IsEdge(maze, coordinate))
            result.Merge(new SearchResult { Best = new Path { Coordinate = coordinate } });

        ++result.CommonDistance;
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
