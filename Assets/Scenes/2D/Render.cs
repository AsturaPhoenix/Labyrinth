using UnityEngine;
using UnityEngine.Tilemaps;

namespace Labyrinth2D
{
    [ExecuteAlways]
    public class Render : MonoBehaviour, Game
    {
        public TileBase[] Tiles;
        public int[] Dimensions;

        public Controller Controller;
        public GameObject SettingsMenu;

        private Tilemap tilemap;

        int[] Game.Dimensions => Dimensions;

        public void NewMaze()
        {
            tilemap.ClearAllTiles();
            Regenerate();
        }

        public GameObject Settings()
        {
            var settings = Instantiate(SettingsMenu);
            settings.GetComponent<Settings>().Game = this;
            return settings;
        }

        public void Export() {
        }

        public bool Import()
        {
            return false;
        }

        private void Regenerate()
        {
            var maze = new Maze(Dimensions);

            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, maze.Dimensions[0] / 2, 0);

            for (int y = 0; y <= maze.Dimensions[1]; ++y)
            {
                for (int x = 0; x <= maze.Dimensions[0]; ++x)
                {
                    RenderCell(maze, x, y);
                }
            }
            
            Controller.Stop();
            Controller.Maze = maze;
            Controller.Position = new Vector2 { x = maze.Entrance[0], y = maze.Entrance[1] };
        }

        void Start()
        {
            tilemap = GetComponent<Tilemap>();
            Regenerate();
        }

        public static int IntersectionType(Maze maze, int x, int y) =>
            (maze[x, y][1] ? 1 : 0) |
            (maze[x, y - 1][0] ? 2 : 0) |
            (maze[x - 1, y][1] ? 4 : 0) |
            (maze[x, y][0] ? 8 : 0);

        private void RenderCell(Maze maze, int x, int y)
        {
            tilemap.SetTile(new Vector3Int(2 * x, -2 * y, 0), Tiles[IntersectionType(maze, x, y)]);
            if (x < maze.Dimensions[0])
                tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y, 0), Tiles[maze[x, y][1] ? 5 : 0]);
            if (y < maze.Dimensions[1])
            {
                tilemap.SetTile(new Vector3Int(2 * x, -2 * y - 1, 0), Tiles[maze[x, y][0] ? 10 : 0]);
                if (x < maze.Dimensions[0])
                    tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y - 1, 0), Tiles[0]);
            }
        }
    }
}
