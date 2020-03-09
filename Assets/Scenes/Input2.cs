using System.Collections.Generic;
using UnityEngine;

public static class Input2
{
    // https://forum.unity.com/threads/mouse-sensitivity-in-webgl-way-too-sensitive.411574/
    // https://issuetracker.unity3d.com/issues/mouse-sensitivity-is-greater-in-webgl-than-in-the-editor
    public static float GetAxis(string axis)
    {
        return (Application.platform == RuntimePlatform.WebGLPlayer ? .5f : 1) * Input.GetAxis(axis);
    }

    public static Vector2 Average(this IEnumerable<Vector2> vectors)
    {
        Vector2 sum = new Vector2();
        int count = 0;
        foreach (var vector in vectors)
        {
            sum += vector;
            ++count;
        }
        return count > 0 ? sum / count : Vector2.zero;
    }
}
