using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth3D
{
    public class Render : MonoBehaviour
    {
        public int Width, Height, Depth;
        public GameObject Wall, Edge, Corner, Player, Entrance, Exit;

        private static Vector3 WallSpace(float x, float y, float z) => new Vector3(x, -y, z);
        private static Vector3 CellSpace(IList<int> coordinate) => WallSpace(coordinate[0] + .5f, coordinate[1] + .5f, coordinate[2] + .5f);

        private Quaternion FaceMaze(IList<int> coordinate)
        {
            if (coordinate[0] < 0)
                return Quaternion.AngleAxis(90, Vector3.up);
            else if (coordinate[0] >= Width)
                return Quaternion.AngleAxis(-90, Vector3.up);
            else if (coordinate[1] < 0)
                return Quaternion.AngleAxis(90, Vector3.right);
            else if (coordinate[1] >= Height)
                return Quaternion.AngleAxis(-90, Vector3.right);
            else if (coordinate[2] >= Depth)
                return Quaternion.AngleAxis(180, Vector3.up);
            else
                return Quaternion.identity;
        }

        void Start()
        {
            var maze = new Maze(Width, Height, Depth);

            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, Width / 2, Height / 2, 0);

            Entrance.transform.localPosition = CellSpace(maze.Entrance);
            Entrance.transform.localRotation = FaceMaze(maze.Entrance);
            Exit.transform.localPosition = CellSpace(maze.Exit);
            Exit.transform.localRotation = FaceMaze(maze.Exit);

            Player.transform.localPosition = Entrance.transform.localPosition;
            Player.transform.localRotation = Entrance.transform.localRotation;

            for (int z = 0; z <= maze.Dimensions[2]; ++z)
            {
                for (int y = 0; y <= maze.Dimensions[1]; ++y)
                {
                    for (int x = 0; x <= maze.Dimensions[0]; ++x)
                    {
                        if (maze[x, y, z][0])
                        {
                            var transform = Instantiate(Wall).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x, y + .5f, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                        }
                        if (maze[x, y, z][1])
                        {
                            var transform = Instantiate(Wall).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x + .5f, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                        }
                        if (maze[x, y, z][2])
                        {
                            var transform = Instantiate(Wall).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x + .5f, y + .5f, z);
                        }

                        if (x < Width)
                        {
                            var transform = Instantiate(Edge).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x + .5f, y, z);
                        }
                        if (y < Height)
                        {
                            var transform = Instantiate(Edge).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x, y + .5f, z);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                        }
                        if (z < Depth)
                        {
                            var transform = Instantiate(Edge).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        }

                        {
                            var transform = Instantiate(Corner).transform;
                            transform.parent = this.transform;
                            transform.localPosition = WallSpace(x, y, z);
                        }
                    }
                }
            }
        }
    }
}
