using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class Controller2DTest
    {
        [Test]
        public void PrimeAtIntersectionBoundary()
        {
            var o = new GameObject();
            var controller = o.AddComponent<Labyrinth2D.Controller>();
            var maze = controller.Maze = new Maze(3, 2);
            controller.Speed = 1 / Time.fixedDeltaTime;

            Maze.Walls walls = maze[1, 1];
            walls[0] = false;
            walls[1] = false;
            walls[2] = false;
            
            controller.Position = new Vector2(.75f, 1);
            controller.EnqueueDirection(1);

            controller.FixedUpdate();

            Assert.AreEqual(1, controller.Position.x);
            Assert.Less(controller.Position.y, 1);
        }
    }
}
