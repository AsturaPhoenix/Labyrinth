using System.Collections.Generic;
using UnityEngine;

public class Render3D2D : MonoBehaviour
{
    private const float Z_PEACEMAKER = .99995f;

    public int Width, Height;
    public float WallHeight, WallThickness;
    public GameObject Ground, Wall, Player, Entrance, Exit;

    private Vector3 WallSpace(float x, float y) => new Vector3(x - Width / 2.0f, 0, Height / 2.0f - y);
    private Vector3 CellSpace(IList<int> coordinate) => WallSpace(coordinate[0] + .5f, coordinate[1] + .5f);

    private Quaternion FaceMaze(IList<int> coordinate)
    {
        float angle;

        if (coordinate[0] < 0)
            angle = 90;
        else if (coordinate[0] >= Width)
            angle = -90;
        else if (coordinate[1] < 0)
            angle = 180;
        else
            angle = 0;

        return Quaternion.AngleAxis(angle, Vector3.up);
    }

    void Start()
    {
        var maze = new Maze(Width, Height);

        DisjointSetMazeGenerator.Generate(maze);
        LongestPathEndpointGenerator.Generate(maze, Width / 2, 0);

        Entrance.transform.localPosition = CellSpace(maze.Entrance);
        Entrance.transform.localRotation = FaceMaze(maze.Entrance);
        Exit.transform.localPosition = CellSpace(maze.Exit);
        Exit.transform.localRotation = FaceMaze(maze.Exit);

        Player.transform.localPosition = Entrance.transform.localPosition;
        Player.transform.localRotation = Entrance.transform.localRotation;

        Ground.transform.localScale = new Vector3(Width, 1, Height);
        var groundRenderer = Ground.GetComponentInChildren<Renderer>();
        foreach (var material in groundRenderer.materials)
        {
            foreach (int id in material.GetTexturePropertyNameIDs())
            {
                material.SetTextureScale(id, new Vector2(Width, Height));
            }
        }

        for (int y = 0; y <= maze.Dimensions[1]; ++y)
        {
            int wallStart = -1;
            for (int x = 0; x <= maze.Dimensions[0]; ++x)
            {
                if ((wallStart == -1) == maze[x, y][1])
                {
                    if (wallStart == -1)
                    {
                        wallStart = x;
                    }
                    else
                    {
                        CreateWall(x - wallStart).transform.localPosition = WallSpace((wallStart + x) / 2.0f, y);
                        wallStart = -1;
                    }
                }
            }
        }

        for (int x = 0; x <= maze.Dimensions[0]; ++x)
        {
            int wallStart = -1;
            for (int y = 0; y <= maze.Dimensions[1]; ++y)
            {
                if ((wallStart == -1) == maze[x, y][0])
                {
                    if (wallStart == -1)
                    {
                        wallStart = y;
                    }
                    else
                    {
                        var transform = CreateWall(y - wallStart).transform;
                        transform.localScale = new Vector3(1, Z_PEACEMAKER, 1);
                        transform.localPosition = WallSpace(x, (wallStart + y) / 2.0f);
                        transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        wallStart = -1;
                    }
                }
            }
        }
    }

    private GameObject CreateWall(int length)
    {
        var wall = new GameObject();
        var geometry = Instantiate(Wall).transform;
        geometry.parent = wall.transform;
        geometry.localScale = new Vector3((length + WallThickness) * Z_PEACEMAKER, WallHeight, WallThickness);
        return wall;
    }
}
