using NUnit.Framework;

namespace Tests
{
    public class MazeSerializerTest
    {
        [Test]
        public void Box2DSingleCell()
        {
            Assert.AreEqual("┌─┐\n" +
                            "└─┘", MazeSerializer.BoxDrawing.Serialize2D(new Maze(1, 1)));
        }

        [Test]
        public void Box2DSpiral()
        {
            var maze = new Maze(3, 3);
            maze[0, 0][2] = false;
            maze[1, 0][2] = false;
            maze[2, 0][3] = false;
            maze[2, 1][3] = false;
            maze[2, 2][0] = false;
            maze[1, 2][0] = false;
            maze[0, 2][1] = false;
            maze[0, 1][2] = false;

            Assert.AreEqual("┌─────┐\n" +
                            "├───┐ │\n" +
                            "│ ╶─┘ │\n" +
                            "└─────┘", MazeSerializer.BoxDrawing.Serialize2D(maze));
        }

        [Test]
        public void Box2DRandom()
        {
            var maze = new Maze(20, 10);
            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, 0, 0);
            Assert.AreEqual(maze, MazeSerializer.BoxDrawing.Deserialize2D(MazeSerializer.BoxDrawing.Serialize2D(maze)));
        }

        [Test]
        public void Box3DRandom()
        {
            var maze = new Maze(5, 6, 7);
            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, 0, 0, 0);
            Assert.AreEqual(maze, MazeSerializer.BoxDrawing.Deserialize3D(MazeSerializer.BoxDrawing.Serialize3D(maze)));
        }
    }
}
