using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MazeSerializer
{
    public static class BoxDrawing
    {
        private const string corners = " ╶╵└╴─┘┴╷┌│├┐┬┤┼", floors = "↑↓↕", reverseEndpoints = "exit < entrance";

        private static bool EndpointsReversed(Maze maze)
        {
            if (maze.Entrance == null || maze.Exit == null) return false;

            for (int i = maze.Dimensions.Dimensionality - 1; i >= 0; --i)
            {
                if (maze.Exit[i] < maze.Entrance[i]) return true;
                if (maze.Exit[i] > maze.Entrance[i]) return false;
            }
            return false;
        }

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

            if (EndpointsReversed(maze))
                sb.Append('\n').Append(reverseEndpoints);

            return sb.ToString();
        }

        public static Maze Deserialize2D(string serialized)
        {
            try
            {
                var rawLines = serialized.Split('\n', '\r');
                var lines = new List<string>(rawLines.Length);
                int width = 0;
                foreach (var line in rawLines.Select(s => s.Trim()).Where(s => s != ""))
                {
                    var lineWidth = (line.Length - 1) / 2;
                    if (lineWidth > width) width = lineWidth;
                    lines.Add(line);
                }

                bool reverseEndpoints = false;
                if (lines[lines.Count - 1] == BoxDrawing.reverseEndpoints)
                {
                    reverseEndpoints = true;
                    lines.RemoveAt(lines.Count - 1);
                }

                var maze = new Maze(width, lines.Count - 1);

                // Walls
                for (int y = 0; y < maze.Dimensions[1]; ++y)
                {
                    var line = lines[y];
                    for (int x = 0; 2 * x < line.Length - 1; ++x)
                    {
                        var wallBits = corners.IndexOf(line[2 * x]);
                        var walls = maze[x, y];
                        if (x > 0) walls[0] = (wallBits & 8) != 0;
                        if (y > 0) walls[1] = (wallBits & 1) != 0;
                    }
                }

                // Endpoints
                int i = 0;
                var endpoints = new ImmutableVector<int>[2];
                {
                    var line = lines[0];
                    for (int x = 0; x < (line.Length - 1) / 2; ++x)
                    {
                        if (line[2 * x + 1] == ' ')
                            endpoints[i++] = new ImmutableVector<int>(x, -1);
                    }
                }
                for (int y = 0; y < maze.Dimensions[1]; ++y)
                {
                    var line = lines[y];
                    if ((corners.IndexOf(line[0]) & 8) == 0)
                        endpoints[i++] = new ImmutableVector<int>(-1, y);
                    if (line.Length <= 2 * maze.Dimensions[0] || (corners.IndexOf(line[2 * maze.Dimensions[0]]) & 8) == 0)
                        endpoints[i++] = new ImmutableVector<int>(maze.Dimensions[0], y);
                }
                {
                    var line = lines[maze.Dimensions[1]];
                    for (int x = 0; x < (line.Length - 1) / 2; ++x)
                    {
                        if (line[2 * x + 1] == ' ')
                            endpoints[i++] = new ImmutableVector<int>(x, maze.Dimensions[1]);
                    }
                }

                switch (i)
                {
                    case 1:
                        maze.Entrance = maze.Exit = endpoints[0];
                        break;
                    case 2:
                        maze.Entrance = endpoints[reverseEndpoints ? 1 : 0];
                        maze.Exit = endpoints[reverseEndpoints ? 0 : 1];
                        break;
                }

                return maze;
            }
            catch
            {
                return null;
            }
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

            if (EndpointsReversed(maze))
                sb.Append('\n').Append(reverseEndpoints);

            return sb.ToString();
        }
    }
}
