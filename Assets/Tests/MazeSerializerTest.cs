using NUnit.Framework;

namespace Tests
{
    public class MazeSerializerTest
    {
        [Test]
        public void Box2DSingleCell()
        {
            Assert.AreEqual("┌─┐ \n" +
                            "└─┘ \n", MazeSerializer.BoxDrawing.Serialize2D(new Maze(1, 1)));
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

            Assert.AreEqual("┌─────┐ \n" +
                            "├───┐ │ \n" +
                            "│ ╶─┘ │ \n" +
                            "└─────┘ \n", MazeSerializer.BoxDrawing.Serialize2D(maze));
        }
    }
}
