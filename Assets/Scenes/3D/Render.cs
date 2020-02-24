using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth3D
{
    public class Render : MonoBehaviour, Game
    {
        public int[] Dimensions;
        public GameObject Wall, Edge, Corner, Entrance, Exit, SettingsMenu;
        public Rigidbody Player;

        private Maze maze;
        private GameObject mazeElements;

        private static Vector3 WallSpace(float x, float y, float z) => new Vector3(x, -y, z);
        private static Vector3 CellSpace(IList<int> coordinate) => WallSpace(coordinate[0] + .5f, coordinate[1] + .5f, coordinate[2] + .5f);

        private Quaternion FaceMaze(IList<int> coordinate)
        {
            switch (maze.IngressDirection(coordinate))
            {
                case 0:
                    return Quaternion.AngleAxis(-90, Vector3.up);
                case 1:
                    return Quaternion.AngleAxis(-90, Vector3.right);
                case 2:
                    return Quaternion.AngleAxis(180, Vector3.up);
                case 3:
                    return Quaternion.AngleAxis(90, Vector3.up);
                case 4:
                    return Quaternion.AngleAxis(90, Vector3.right);
                default:
                    return Quaternion.identity;
            }
        }

        int[] Game.Dimensions => Dimensions;

        public void NewMaze()
        {
            Destroy(mazeElements);
            Player.velocity = Vector3.zero;
            Start();
        }

        public GameObject Settings()
        {
            var settings = Instantiate(SettingsMenu);
            settings.GetComponent<Settings>().Game = this;
            return settings;
        }

        public void Export() { }

        public bool Import()
        {
            return false;
        }

        private Transform MazeElement(GameObject template) => Instantiate(template, mazeElements.transform).transform;

        void Start()
        {
            mazeElements = new GameObject();
            mazeElements.transform.parent = transform;

            maze = new Maze(Dimensions);

            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, maze.Dimensions[0] / 2, maze.Dimensions[1] / 2, 0);

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
                            var transform = MazeElement(Wall);
                            transform.localPosition = WallSpace(x, y + .5f, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                        }
                        if (maze[x, y, z][1])
                        {
                            var transform = MazeElement(Wall);
                            transform.localPosition = WallSpace(x + .5f, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                        }
                        if (maze[x, y, z][2])
                        {
                            MazeElement(Wall).localPosition = WallSpace(x + .5f, y + .5f, z);
                        }

                        if (x < maze.Dimensions[0])
                        {
                            MazeElement(Edge).localPosition = WallSpace(x + .5f, y, z);
                        }
                        if (y < maze.Dimensions[1])
                        {
                            var transform = MazeElement(Edge);
                            transform.localPosition = WallSpace(x, y + .5f, z);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                        }
                        if (z < maze.Dimensions[2])
                        {
                            var transform = MazeElement(Edge);
                            transform.localPosition = WallSpace(x, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        }

                        if (Corner != null)
                        {
                            MazeElement(Corner).localPosition = WallSpace(x, y, z);
                        }
                    }
                }
            }
        }
    }
}
