using UnityEngine;
using UnityEngine.Tilemaps;

public class Render2D : MonoBehaviour
{
    public TileBase[] Tiles;

    void Start()
    {
        var tilemap = GetComponent<Tilemap>();
        var maze = new Maze(10, 10);

        for (int y = 0; y <= maze.GetDimension(1); ++y)
        {
            for (int x = 0; x <= maze.GetDimension(0); ++x)
            {
                RenderCell(tilemap, x, y, maze);
            }
        }
    }

    private void RenderCell(Tilemap tilemap, int x, int y, Maze maze)
    {
        bool[] localWalls = maze.GetWalls(x, y);
        bool prevLeft = maze.GetWalls(x, y - 1)[0], prevTop = maze.GetWalls(x - 1, y)[1];

        int upperLeftWall = (localWalls[1] ? 1 : 0) |
            (prevLeft ? 2 : 0) |
            (prevTop ? 4 : 0) |
            (localWalls[0] ? 8 : 0);
        tilemap.SetTile(new Vector3Int(2 * x, -2 * y, 0), Tiles[upperLeftWall]);
        if (x < maze.GetDimension(0))
            tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y, 0), Tiles[localWalls[1] ? 5 : 0]);
        if (y < maze.GetDimension(1))
        {
            tilemap.SetTile(new Vector3Int(2 * x, -2 * y - 1, 0), Tiles[localWalls[0] ? 10 : 0]);
            if (x < maze.GetDimension(0))
                tilemap.SetTile(new Vector3Int(2 * x + 1, -2 * y - 1, 0), Tiles[0]);
        }
    }
}
