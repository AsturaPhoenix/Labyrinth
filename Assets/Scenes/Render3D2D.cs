using System.Collections.Generic;
using UnityEngine;

public class Render3D2D : MonoBehaviour
{
    public int Width, Height;
    public float WallHeight, WallThickness;
    public GameObject Ground, WallPlane, Player, Entrance, Exit;

    private List<GameObject> walls = new List<GameObject>();

    private Vector3 WallSpace(float x, float y) => new Vector3(x - Width / 2.0f, 0, Height / 2.0f - y);
    private Vector3 CellSpace(IList<int> coordinate) => WallSpace(coordinate[0] + .5f, coordinate[1] + .5f);

    private struct WallBevel
    {
        public bool LeftNear, LeftFar, RightNear, RightFar;
    }

    void Start()
    {
        var maze = new Maze(Width, Height);

        DisjointSetMazeGenerator.Generate(maze);
        LongestPathEndpointGenerator.Generate(maze, Width / 2, 0);

        Entrance.transform.localPosition = CellSpace(maze.Entrance);
        Exit.transform.localPosition = CellSpace(maze.Exit);

        Player.transform.localPosition = CellSpace(maze.Entrance);
        if (maze.Entrance[0] < 0)
        {
            Player.transform.Rotate(0, 90, 0);
        }
        else if (maze.Entrance[0] >= Width)
        {
            Player.transform.Rotate(0, -90, 0);
        }
        else if (maze.Entrance[1] < 0)
        {
            Player.transform.Rotate(0, 180, 0);
        }

        Ground.transform.localScale = new Vector3(Width, 1, Height);

        for (int y = 0; y <= maze.Dimensions[1]; ++y)
        {
            int wallStart = -1;
            WallBevel bevel = new WallBevel();
            for (int x = 0; x <= maze.Dimensions[0]; ++x)
            {
                if ((wallStart == -1) == maze[x, y][1])
                {
                    if (wallStart == -1)
                    {
                        wallStart = x;
                        bevel.LeftNear = maze[x, y][0];
                        bevel.LeftFar = maze[x, y - 1][0];
                    }
                    else
                    {
                        bevel.RightNear = maze[x, y][0];
                        bevel.RightFar = maze[x, y - 1][0];
                        CreateWall(x - wallStart, bevel).transform.localPosition = WallSpace((wallStart + x) / 2.0f, y);
                        wallStart = -1;
                    }
                }
            }
        }

        for (int x = 0; x <= maze.Dimensions[0]; ++x)
        {
            int wallStart = -1;
            WallBevel bevel = new WallBevel();
            for (int y = 0; y <= maze.Dimensions[1]; ++y)
            {
                if ((wallStart == -1) == maze[x, y][0])
                {
                    if (wallStart == -1)
                    {
                        wallStart = y;
                        bevel.RightNear = maze[x, y][1];
                        bevel.RightFar = maze[x - 1, y][1];
                    }
                    else
                    {
                        bevel.LeftNear = maze[x, y][1];
                        bevel.LeftFar = maze[x - 1, y][1];
                        var transform = CreateWall(y - wallStart, bevel).transform;
                        transform.localPosition = WallSpace(x, (wallStart + y) / 2.0f);
                        transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        wallStart = -1;
                    }
                }
            }
        }
    }

    private GameObject CreateWall(int length, WallBevel bevel)
    {
        var wall = new GameObject();
        wall.transform.parent = gameObject.transform;

        float t2 = WallThickness / 2, t4 = WallThickness / 4;

        var near = Instantiate(WallPlane).transform;
        near.parent = wall.transform;
        near.localPosition = new Vector3((bevel.LeftNear ? t4 : -t4) +
                                         (bevel.RightNear ? -t4 : t4), 0, -t2);
        near.localScale = new Vector3((bevel.LeftNear ? -t2 : t2) +
                                      (bevel.RightNear ? -t2 : t2) +
                                      length, WallHeight, 1);

        var far = Instantiate(WallPlane).transform;
        far.parent = wall.transform;
        far.localPosition = new Vector3((bevel.LeftFar ? t4 : -t4) +
                                        (bevel.RightFar ? -t4 : t4), 0, t2);
        far.localScale = new Vector3((bevel.LeftFar ? -t2 : t2) +
                                     (bevel.RightFar ? -t2 : t2) +
                                     length, WallHeight, 1);
        far.Rotate(0, 180, 0);

        if (!(bevel.LeftNear || bevel.LeftFar))
        {
            var left = Instantiate(WallPlane).transform;
            left.parent = wall.transform;
            left.localPosition = new Vector3(-length / 2.0f - t2, 0, 0);
            left.localScale = new Vector3(WallThickness, WallHeight, 1);
            left.Rotate(0, 90, 0);
        }

        if (!(bevel.RightNear || bevel.RightFar))
        {
            var right = Instantiate(WallPlane).transform;
            right.parent = wall.transform;
            right.localPosition = new Vector3(length / 2.0f + t2, 0, 0);
            right.localScale = new Vector3(WallThickness, WallHeight, 1);
            right.Rotate(0, -90, 0);
        }

        walls.Add(wall);

        return wall;
    }
}
