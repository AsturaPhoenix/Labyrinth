using UnityEngine;
using UnityEngine.UI;

public class Export : MonoBehaviour, Submenu
{
    public MenuStack Menu;
    public InputField Serialized;

    MenuStack Submenu.Menu { set => Menu = value; }

    private void Start()
    {
        var textComponent = Serialized.textComponent;
        var settings = textComponent.GetGenerationSettings(new Vector2(Serialized.preferredWidth, Serialized.minHeight));
        var gen = textComponent.cachedTextGeneratorForLayout;
        var text = Serialized.text;
        var rectTransform = Serialized.GetComponent<RectTransform>();
        var textRect = textComponent.rectTransform.rect;
        rectTransform.sizeDelta = new Vector2(gen.GetPreferredWidth(text, settings), gen.GetPreferredHeight(text, settings))
            + textRect.min + rectTransform.rect.size - textRect.max;
    }

    public void OnBack()
    {
        Menu.Pop();
    }
}
