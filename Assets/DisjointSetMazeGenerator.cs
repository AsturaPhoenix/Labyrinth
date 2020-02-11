using System.Collections.Generic;
using UnityEngine;

public static class DisjointSetMazeGenerator
{
    private class Cell
    {
        public Cell Subtree;
        public Maze.Walls Walls;

        public Cell(Maze.Walls walls)
        {
            Subtree = this;
            Walls = walls;
        }
    }

    private class Wall
    {
        public Cell owner;
        public int side;
        public Cell other;
    }

    public static void Generate(Maze maze)
    {
        var cells = new CartesianField<Cell>(maze.Dimensions, coordinate => new Cell(maze[coordinate]));
        var walls = new List<Wall>(maze.Volume * maze.Dimensions.Dimensionality);

        foreach (var coordinate in cells.Coordinates)
        {
            for (int i = 0; i < coordinate.Dimensionality; ++i)
            {
                if (coordinate[i] > 0)
                {
                    walls.Add(new Wall
                    {
                        owner = cells[coordinate],
                        side = i,
                        other = cells[Maze.GetNeighbor(coordinate, i)]
                    });
                }
            }
        }

        // eliminate n - 1 walls
        for (int c = 1; c < maze.Volume; ++c)
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

    private static Cell ResolveSubtree(Cell cell)
    {
        while (cell.Subtree != cell)
        {
            cell.Subtree = cell.Subtree.Subtree;
            cell = cell.Subtree;
        }
        return cell;
    }
}
