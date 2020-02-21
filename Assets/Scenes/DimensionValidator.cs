using System;
using UnityEngine;
using UnityEngine.UI;

public class DimensionValidator : MonoBehaviour
{
    public static char Validate(string text, int charIndex, char addedChar) =>
        addedChar >= '1' && addedChar <= '9' || charIndex != 0 && addedChar == '0' ? addedChar : '\0';

    public static bool Validate(string text)
    {
        int dim;
        return int.TryParse(text, out dim) && dim > 0;
    }

    public static bool Validate(params InputField[] fields) => Array.TrueForAll(fields, field => Validate(field.text));

    private void Start()
    {
        GetComponent<InputField>().onValidateInput += Validate;
    }
}
