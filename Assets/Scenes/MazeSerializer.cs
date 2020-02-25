using System.Text;
using UnityEngine;

public static class MazeSerializer
{
    public static class BoxDrawing
    {
        private const string corners = " ╶╵└╴─┘┴╷┌│├┐┬┤┼", floors = "↑↓↕";

        public static string Serialize2D(Maze maze)
        {
            Debug.Assert(maze.Dimensions.Dimensionality == 2);

            var sb = new StringBuilder();

            for (int y = 0; y <= maze.Dimensions[1]; ++y)
            {
                for (int x = 0; x < maze.Dimensions[0]; ++x)
                {
                    sb.Append(corners[Labyrinth2D.Render.IntersectionType(maze, x, y)]);
                    sb.Append(maze[x, y][1] ? '─' : ' ');
                }
                sb.Append(corners[Labyrinth2D.Render.IntersectionType(maze, maze.Dimensions[0], y)]);
                if (y < maze.Dimensions[1])
                    sb.Append('\n');
            }

            return sb.ToString();
        }

        public static string Serialize3D(Maze maze)
        {
            Debug.Assert(maze.Dimensions.Dimensionality == 3);

            var sb = new StringBuilder();

            for (int z = maze.Dimensions[2] - 1; z >= -1; --z)
            {
                for (int y = maze.Dimensions[1] - 1; y >= 0; --y)
                {
                    int IntersectionType(int x) =>
                        (maze[x, y, z][5] ? 1 : 0) |
                        (maze[x, y, z + 1][0] ? 2 : 0) |
                        (maze[x - 1, y, z][5] ? 4 : 0) |
                        (maze[x, y, z][0] ? 8 : 0);

                    for (int x = 0; x < maze.Dimensions[0]; ++x)
                    {
                        sb.Append(corners[IntersectionType(x)]);
                        
                        bool upPath = !maze[x, y, z][1] && z >= 0,
                             downPath = !maze[x, y, z + 1][4] && z + 1 < maze.Dimensions[2];
                        if (upPath || downPath)
                        {
                            int floorType = ((upPath ? 1 : 0) | (downPath ? 2 : 0)) - 1;
                            sb.Append(floors[floorType]);
                        }
                        else
                        {
                            sb.Append(maze[x, y, z][5] ? '─' : ' ');
                        }
                    }
                    sb.Append(corners[IntersectionType(maze.Dimensions[0])]);
                }
                if (z >= 0)
                    sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
