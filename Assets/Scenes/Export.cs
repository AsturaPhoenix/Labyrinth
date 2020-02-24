using UnityEngine;
using UnityEngine.UI;

public class Export : MonoBehaviour, Submenu
{
    public MenuStack Menu;
    public InputField Serialized;

    MenuStack Submenu.Menu { set => Menu = value; }

    public void OnBack()
    {
        Menu.Pop();
    }
}
