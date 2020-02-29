using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class MazeSerializer
{
    public static class BoxDrawing
    {
        private const string corners = " ╶╵└╴─┘┴╷┌│├┐┬┤┼", floors = "↑↓↕", reverseEndpoints = "exit < entrance";
        private static int DecodeCorner(char corner) => corners.IndexOf(corner);
        private static int DecodeFloor(char floor) => floors.IndexOf(floor) + 1;

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
                foreach (var line in from s in rawLines where s != "" select s)
                    lines.Add(line);

                bool reverseEndpoints = false;
                if (lines[lines.Count - 1] == BoxDrawing.reverseEndpoints)
                {
                    reverseEndpoints = true;
                    lines.RemoveAt(lines.Count - 1);
                }

                var maze = new Maze((lines[0].Length - 1) / 2, lines.Count - 1);

                // Walls
                for (int y = 0; y < maze.Dimensions[1]; ++y)
                {
                    var line = lines[y];
                    for (int x = 0; x < maze.Dimensions[0]; ++x)
                    {
                        var wallBits = DecodeCorner(line[2 * x]);
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
                    for (int x = 0; x < maze.Dimensions[0]; ++x)
                    {
                        if (line[2 * x + 1] == ' ')
                            endpoints[i++] = new ImmutableVector<int>(x, -1);
                    }
                }
                for (int y = 0; y < maze.Dimensions[1]; ++y)
                {
                    var line = lines[y];
                    if ((DecodeCorner(line[0]) & 8) == 0)
                        endpoints[i++] = new ImmutableVector<int>(-1, y);
                    if ((DecodeCorner(line[2 * maze.Dimensions[0]]) & 8) == 0)
                        endpoints[i++] = new ImmutableVector<int>(maze.Dimensions[0], y);
                }
                {
                    var line = lines[maze.Dimensions[1]];
                    for (int x = 0; x < maze.Dimensions[0]; ++x)
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

        public static Maze Deserialize3D(string serialized)
        {
            try
            {
                var rawLines = serialized.Split('\n', '\r');
                var lines = new List<string>(rawLines.Length);
                foreach (var line in from s in rawLines where s != "" select s)
                    lines.Add(line);

                bool reverseEndpoints = false;
                if (lines[lines.Count - 1] == BoxDrawing.reverseEndpoints)
                {
                    reverseEndpoints = true;
                    lines.RemoveAt(lines.Count - 1);
                }

                int layerWidth = 3;
                bool HasVerticalWall()
                {
                    for (int row = 0; row < lines.Count; ++row)
                    {
                        int corner = DecodeCorner(lines[row][layerWidth]);
                        if (corner != -1 && (corner & 10) != 0)
                            return true;
                    }
                    return false;
                }
                while (!HasVerticalWall()) layerWidth += 2;

                var maze = new Maze((layerWidth - 1) / 2, lines[0].Length / layerWidth, lines.Count - 1);

                // Walls
                for (int zi = 0; zi < maze.Dimensions[2]; ++zi)
                {
                    int z = maze.Dimensions[2] - 1 - zi;
                    for (int yi = 0; yi < maze.Dimensions[1]; ++yi)
                    {
                        int y = maze.Dimensions[1] - 1 - yi;
                        var line = lines[zi];
                        for (int x = 0; x < maze.Dimensions[0]; ++x)
                        {
                            int ci = layerWidth * yi + 2 * x;

                            var wallBits = DecodeCorner(line[ci]);
                            var walls = maze[x, y, z];
                            if (x > 0) walls[0] = (wallBits & 8) != 0;
                            if (zi > 0) walls[5] = (wallBits & 1) != 0;

                            if (y > 0) walls[1] = (DecodeFloor(line[ci + 1]) & 1) == 0;
                            if (yi > 0) walls[4] = (DecodeFloor(lines[zi + 1][ci + 1]) & 2) == 0;
                        }
                    }
                }

                // Endpoints
                int i = 0;
                var endpoints = new ImmutableVector<int>[2];
                {
                    var line = lines[maze.Dimensions[2]];
                    for (int y = 0; y < maze.Dimensions[1]; ++y)
                    {
                        int yi = maze.Dimensions[1] - 1 - y;
                        for (int x = 0; x < maze.Dimensions[0]; ++x)
                        {
                            if ((DecodeCorner(line[layerWidth * yi + 2 * x]) & 1) == 0)
                                endpoints[i++] = new ImmutableVector<int>(x, y, -1);
                        }
                    }
                }
                for (int z = 0; z < maze.Dimensions[2]; ++z)
                {
                    var line = lines[maze.Dimensions[2] - 1 - z];
                    {
                        int ci = layerWidth * (maze.Dimensions[1] - 1);
                        for (int x = 0; x < maze.Dimensions[0]; ++x)
                        {
                            if ((DecodeFloor(line[ci + 2 * x + 1]) & 1) != 0)
                                endpoints[i++] = new ImmutableVector<int>(x, -1, z);
                        }
                    }
                    for (int y = 0; y < maze.Dimensions[1]; ++y)
                    {
                        int ci = layerWidth * (maze.Dimensions[1] - 1 - y);
                        if ((DecodeCorner(line[ci]) & 8) == 0)
                            endpoints[i++] = new ImmutableVector<int>(-1, y, z);
                        if ((DecodeCorner(line[ci + layerWidth - 1]) & 8) == 0)
                            endpoints[i++] = new ImmutableVector<int>(maze.Dimensions[0], y, z);
                    }
                    line = lines[maze.Dimensions[2] - z];
                    {
                        for (int x = 0; x < maze.Dimensions[0]; ++x)
                        {
                            if ((DecodeFloor(line[2 * x + 1]) & 2) != 0)
                                endpoints[i++] = new ImmutableVector<int>(x, maze.Dimensions[1], z);
                        }
                    }
                }
                {
                    var line = lines[0];
                    for (int y = 0; y < maze.Dimensions[1]; ++y)
                    {
                        int yi = maze.Dimensions[1] - 1 - y;
                        for (int x = 0; x < maze.Dimensions[0]; ++x)
                        {
                            if ((DecodeCorner(line[layerWidth * yi + 2 * x]) & 1) == 0)
                                endpoints[i++] = new ImmutableVector<int>(x, y, maze.Dimensions[2]);
                        }
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
            catch (System.Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }
    }
}
