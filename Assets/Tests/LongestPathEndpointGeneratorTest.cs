using NUnit.Framework;

namespace Tests
{
    public class LongestPathEndpointGeneratorTest
    {
        [Test]
        public void SpiralMaze()
        {
            // ┌─────┐ 
            // ├───┐ │ 
            // │ ╶─┘ │ 
            // └─────┘
            var maze = new Maze(3, 3);
            maze[0, 0][2] = false;
            maze[1, 0][2] = false;
            maze[2, 0][3] = false;
            maze[2, 1][3] = false;
            maze[2, 2][0] = false;
            maze[1, 2][0] = false;
            maze[0, 2][1] = false;
            maze[0, 1][2] = false;
            LongestPathEndpointGenerator.Generate(maze, 0, 0);
            
            Assert.AreEqual(new int[] { -1, 1 }, maze.Exit);
        }

        [Test]
        public void LargeMaze()
        {
            var maze = new Maze(10, 10);
            DisjointSetMazeGenerator.Generate(maze);
            LongestPathEndpointGenerator.Generate(maze, 0, 0);
            Assert.NotNull(maze.Entrance);
            Assert.NotNull(maze.Exit);
        }
    }
}
