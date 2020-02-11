using UnityEngine;
using UnityEngine.Tilemaps;

public class Render2D : MonoBehaviour
{
    public TileBase[] Tiles;
    public int Width, Height;

    void Start()
    {
        var tilemap = GetComponent<Tilemap>();
        var maze = new Maze(Width, Height);

        DisjointSetMazeGenerator.Generate(maze);
        maze.Entrance = new ImmutableVector<int>(0, -1);
        maze.Exit = new ImmutableVector<int>(Width - 1, Height);

        for (int y = 0; y <= maze.Dimensions[1]; ++y)
        {
            for (int x = 0; x <= maze.Dimensions[0]; ++x)
            {
                RenderCell(tilemap, x, y, maze);
            }
        }
    }

    private void RenderCell(Tilemap tilemap, int x, int y, Maze maze)
    {
        Maze.Walls localWalls = maze[x, y];
        bool prevLeft = maze[x, y - 1][0], prevTop = maze[x - 1, y][1];

        int upperLeftWall = (localWalls[1] ? 1 : 0) |
            (prevLeft ? 2 : 0) |
            (prevTop ? 4 : 0) |
            (localWalls[0] ? 8 : 0);
        tilemap.SetTile(new Vector3Int(2 * x, -2 * y, 0), Tiles[upperLeftWall]);
        if (x < maze.Dimensions[0])
            tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y, 0), Tiles[localWalls[1] ? 5 : 0]);
        if (y < maze.Dimensions[1])
        {
            tilemap.SetTile(new Vector3Int(2 * x, -2 * y - 1, 0), Tiles[localWalls[0] ? 10 : 0]);
            if (x < maze.Dimensions[0])
                tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y - 1, 0), Tiles[0]);
        }
    }
}
