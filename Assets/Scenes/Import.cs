using UnityEngine;
using UnityEngine.UI;

public class Import : MonoBehaviour, Submenu
{
    public Game Game;
    public MenuStack Menu;
    public InputField Serialized;

    MenuStack Submenu.Menu { set => Menu = value; }

    public void Layout()
    {
        Debug.Log("Layout");
        var textComponent = Serialized.textComponent;
        var settings = textComponent.GetGenerationSettings(new Vector2(Serialized.preferredWidth, Serialized.minHeight));
        var gen = textComponent.cachedTextGeneratorForLayout;
        var text = Serialized.text;
        var textRect = textComponent.rectTransform.rect;
        var rectTransform = Serialized.GetComponent<RectTransform>();
        Vector2 padding = textRect.min + rectTransform.rect.size - textRect.max;
        var layout = Serialized.GetComponent<LayoutElement>();
        layout.preferredWidth = gen.GetPreferredWidth(text, settings) + padding.x;
        layout.preferredHeight = gen.GetPreferredHeight(text, settings) + padding.y;
    }

    public void OnImport()
    {
        if (Game.Import(Serialized.text))
            Menu.Destroy();
    }

    public void OnBack()
    {
        Menu.Pop();
    }
}
