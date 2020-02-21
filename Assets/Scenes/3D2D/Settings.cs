using UnityEngine;
using UnityEngine.UI;

namespace Labyrinth3D2D
{
    public class Settings : MonoBehaviour, Submenu
    {
        public InputField Width, Height;
        public Button Regenerate;

        public MenuStack Menu;
        public Render Maze;

        MenuStack Submenu.Menu { set => Menu = value; }

        private void Start()
        {
            Width.text = Maze.Width.ToString();
            Height.text = Maze.Height.ToString();
        }

        public void Validate()
        {
            Regenerate.interactable = DimensionValidator.Validate(Width, Height);
        }

        public void OnRegenerate()
        {
            Menu.Destroy();

            Maze.Width = int.Parse(Width.text);
            Maze.Height = int.Parse(Height.text);

            Maze.NewMaze();
        }

        public void OnBack()
        {
            Menu.Pop();
        }
    }
}
