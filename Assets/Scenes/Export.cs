﻿using UnityEngine;
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
        var textRect = textComponent.rectTransform.rect;
        var rectTransform = Serialized.GetComponent<RectTransform>();
        Vector2 padding = textRect.min + rectTransform.rect.size - textRect.max;
        var layout = Serialized.GetComponent<LayoutElement>();
        layout.preferredWidth = gen.GetPreferredWidth(text, settings) + padding.x;
        layout.preferredHeight = gen.GetPreferredHeight(text, settings) + padding.y;
    }

    public void OnBack()
    {
        Menu.Pop();
    }
}
