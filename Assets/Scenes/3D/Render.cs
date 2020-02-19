﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth3D
{
    public class Render : MonoBehaviour, Game
    {
        public int Width, Height, Depth;
        public GameObject Wall, Edge, Corner, Player, Entrance, Exit;

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

        public void NewMaze()
        {
            Destroy(mazeElements);
            Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Start();
        }

        public void Settings() { }

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

            maze = new Maze(Width, Height, Depth);

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

                        if (x < Width)
                        {
                            MazeElement(Edge).localPosition = WallSpace(x + .5f, y, z);
                        }
                        if (y < Height)
                        {
                            var transform = MazeElement(Edge);
                            transform.localPosition = WallSpace(x, y + .5f, z);
                            transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
                        }
                        if (z < Depth)
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
