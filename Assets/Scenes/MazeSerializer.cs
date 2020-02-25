using System.Text;
using UnityEngine;

public static class MazeSerializer
{
    public static class BoxDrawing
    {
        private const string corners = " ╶╵└╴─┘┴╷┌│├┐┬┤┼";

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
    }
}
