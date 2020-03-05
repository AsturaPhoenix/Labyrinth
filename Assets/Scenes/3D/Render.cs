using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth3D
{
    public class Render : MonoBehaviour, Game
    {
        public int[] Dimensions;
        public GameObject Wall, Edge, Corner, Entrance, Exit, SettingsMenu;
        public Rigidbody Player;

        private IMaze maze;
        private GameObject mazeElements;
        
        private static Vector3 CellSpace(IList<int> coordinate) => new Vector3(coordinate[0] + .5f, coordinate[1] + .5f, coordinate[2] + .5f);

        private Quaternion FaceMaze(IList<int> coordinate)
        {
            switch (maze.IngressDirection(coordinate))
            {
                case 0:
                    return Quaternion.AngleAxis(-90, Vector3.up);
                case 1:
                    return Quaternion.AngleAxis(90, Vector3.right);
                case 2:
                    return Quaternion.AngleAxis(180, Vector3.up);
                case 3:
                    return Quaternion.AngleAxis(90, Vector3.up);
                case 4:
                    return Quaternion.AngleAxis(-90, Vector3.right);
                default:
                    return Quaternion.identity;
            }
        }

        int[] Game.Dimensions => Dimensions;

        public void NewMaze()
        {
            var maze = new Maze(Dimensions);
            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, maze.Dimensions[0] / 2, maze.Dimensions[1] / 2, 0);
            this.maze = maze;

            MazeUpdated();
        }

        public GameObject Settings()
        {
            var settings = Instantiate(SettingsMenu);
            settings.GetComponent<global::Settings>().Game = this;
            settings.GetComponent<Settings>().Render = this;
            return settings;
        }

        public string Export() => MazeSerializer.BoxDrawing.Serialize3D(maze.Swizzle(0, 1, 5));

        public bool Import(string serialized)
        {
            var newMaze = MazeSerializer.BoxDrawing.Deserialize3D(serialized).Swizzle(0, 1, 5);
            if (newMaze == null)
                return false;

            maze = newMaze;
            MazeUpdated();
            return true;
        }

        private void MazeUpdated()
        {
            Player.velocity = Vector3.zero;

            Entrance.transform.localPosition = CellSpace(maze.Entrance);
            Entrance.transform.localRotation = FaceMaze(maze.Entrance);
            Exit.transform.localPosition = CellSpace(maze.Exit);
            Exit.transform.localRotation = FaceMaze(maze.Exit);

            Player.transform.localPosition = Entrance.transform.localPosition;
            Player.transform.localRotation = Entrance.transform.localRotation;

            UpdateModels();
        }

        private Transform MazeElement(GameObject template) => Instantiate(template, mazeElements.transform).transform;

        public void UpdateModels()
        {
            Destroy(mazeElements);

            mazeElements = new GameObject();
            mazeElements.transform.parent = transform;

            for (int z = 0; z <= maze.Dimensions[2]; ++z)
            {
                for (int y = 0; y <= maze.Dimensions[1]; ++y)
                {
                    for (int x = 0; x <= maze.Dimensions[0]; ++x)
                    {
                        if (maze[x, y, z][0])
                        {
                            var transform = MazeElement(Wall);
                            transform.localPosition = new Vector3(x, y + .5f, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.up);
                        }
                        if (maze[x, y, z][1])
                        {
                            var transform = MazeElement(Wall);
                            transform.localPosition = new Vector3(x + .5f, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                        }
                        if (maze[x, y, z][2])
                        {
                            MazeElement(Wall).localPosition = new Vector3(x + .5f, y + .5f, z);
                        }

                        if (x < maze.Dimensions[0])
                        {
                            MazeElement(Edge).localPosition = new Vector3(x + .5f, y, z);
                        }
                        if (y < maze.Dimensions[1])
                        {
                            var transform = MazeElement(Edge);
                            transform.localPosition = new Vector3(x, y + .5f, z);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                        }
                        if (z < maze.Dimensions[2])
                        {
                            var transform = MazeElement(Edge);
                            transform.localPosition = new Vector3(x, y, z + .5f);
                            transform.localRotation = Quaternion.AngleAxis(-90, Vector3.up);
                        }

                        if (Corner != null)
                        {
                            MazeElement(Corner).localPosition = new Vector3(x, y, z);
                        }
                    }
                }
            }
        }

        private void Start() => NewMaze();
    }
}
