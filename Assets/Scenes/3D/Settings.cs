using UnityEngine;
using UnityEngine.UI;

namespace Labyrinth3D
{
    public class Settings : MonoBehaviour, Submenu
    {
        public InputField Width, Height, Depth;
        public Button Regenerate;

        public MenuStack Menu;
        public Render Maze;

        MenuStack Submenu.Menu { set => Menu = value; }

        private void Start()
        {
            Width.text = Maze.Width.ToString();
            Height.text = Maze.Height.ToString();
            Depth.text = Maze.Depth.ToString();
        }

        public void Validate()
        {
            Regenerate.interactable = DimensionValidator.Validate(Width, Height, Depth);
        }

        public void OnRegenerate()
        {
            Menu.Destroy();

            Maze.Width = int.Parse(Width.text);
            Maze.Height = int.Parse(Height.text);
            Maze.Depth = int.Parse(Depth.text);

            Maze.NewMaze();
        }

        public void OnBack()
        {
            Menu.Pop();
        }
    }
}
