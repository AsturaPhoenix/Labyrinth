﻿using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth3D2D
{
    public class Render : MonoBehaviour, Game
    {
        // TODO: actual mesh generation
        private const float Z_PEACEMAKER = .99995f;

        public int[] Dimensions;
        public float WallHeight, WallThickness;
        public GameObject Ground, Wall, Entrance, Exit, SettingsMenu;
        public Rigidbody Player;

        private IMaze maze;
        private GameObject mazeElements;

        private Vector3 WallSpace(float x, float y) => new Vector3(x, 0, y);
        private Vector3 CellSpace(IList<int> coordinate) => WallSpace(coordinate[0] + .5f, coordinate[1] + .5f);

        private Quaternion FaceMaze(IList<int> coordinate) => Quaternion.AngleAxis(-90 * maze.IngressDirection(coordinate) - 90, Vector3.up);

        int[] Game.Dimensions => Dimensions;

        public void NewMaze()
        {
            var maze = new Maze(Dimensions);
            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, maze.Dimensions[0] / 2, 0);
            this.maze = maze;

            MazeUpdated();
        }
        
        public GameObject Settings()
        {
            var settings = Instantiate(SettingsMenu);
            settings.GetComponent<Settings>().Game = this;
            return settings;
        }

        public string Export() => MazeSerializer.BoxDrawing.Serialize2D(maze.Swizzle(0, 3));

        public bool Import(string serialized)
        {
            var newMaze = MazeSerializer.BoxDrawing.Deserialize2D(serialized).Swizzle(0, 3);
            if (newMaze == null)
                return false;

            maze = newMaze;
            return true;
        }

        private void MazeUpdated()
        {
            Destroy(mazeElements);
            Player.velocity = Vector3.zero;

            mazeElements = new GameObject();
            mazeElements.transform.parent = transform;

            Entrance.transform.localPosition = CellSpace(maze.Entrance);
            Entrance.transform.localRotation = FaceMaze(maze.Entrance);
            Exit.transform.localPosition = CellSpace(maze.Exit);
            Exit.transform.localRotation = FaceMaze(maze.Exit);

            Player.transform.localPosition = Entrance.transform.localPosition;
            Player.transform.localRotation = Entrance.transform.localRotation;

            Ground.transform.localScale = new Vector3(maze.Dimensions[0], 1, maze.Dimensions[1]);
            foreach (var material in Ground.GetComponentInChildren<Renderer>().materials)
            {
                foreach (int id in material.GetTexturePropertyNameIDs())
                {
                    material.SetTextureScale(id, new Vector2(maze.Dimensions[0], maze.Dimensions[1]));
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

        private void Start() => NewMaze();

        private Transform CreateWall(int length)
        {
            var wall = new GameObject().transform;
            wall.parent = mazeElements.transform;

            Instantiate(Wall, wall).transform.localScale = new Vector3((length + WallThickness) * Z_PEACEMAKER, WallHeight, WallThickness);
            return wall;
        }
    }
}
