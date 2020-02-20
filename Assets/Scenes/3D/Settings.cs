using UnityEngine;
using UnityEngine.UI;

namespace Labyrinth3D
{
    public class Settings : MonoBehaviour, Submenu
    {
        public InputField Width, Height, Depth;

        public MenuStack Menu;
        public Render Maze;

        MenuStack Submenu.Menu { set => Menu = value; }

        public void ValidateDimension(InputField inputField)
        {
            if (inputField.text.Length > 0 && (inputField.text[0] == '-' || inputField.text[0] == '0'))
            {
                inputField.text = inputField.text.Substring(1);
                inputField.caretPosition = 0;
            }
        }

        private void Start()
        {
            Width.text = Maze.Width.ToString();
            Height.text = Maze.Height.ToString();
            Depth.text = Maze.Depth.ToString();
        }

        public void Regenerate()
        {
            Menu.Destroy();

            Maze.Width = int.Parse(Width.text);
            Maze.Height = int.Parse(Height.text);
            Maze.Depth = int.Parse(Depth.text);

            Maze.NewMaze();
        }

        public void Back()
        {
            Menu.Pop();
        }
    }
}
