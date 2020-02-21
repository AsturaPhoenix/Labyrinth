using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour, Submenu
{
    public InputField[] Dimensions;
    public Button Regenerate;

    public MenuStack Menu;
    public Game Game;

    MenuStack Submenu.Menu { set => Menu = value; }

    private void Start()
    {
        for (int i = 0; i < Dimensions.Length; ++i)
            Dimensions[i].text = Game.Dimensions[i].ToString();
    }

    public void Validate()
    {
        Regenerate.interactable = DimensionValidator.Validate(Dimensions);
    }

    public void OnRegenerate()
    {
        Menu.Destroy();

        for (int i = 0; i < Dimensions.Length; ++i)
            Game.Dimensions[i] = int.Parse(Dimensions[i].text);

        Game.NewMaze();
    }

    public void OnBack()
    {
        Menu.Pop();
    }
}
