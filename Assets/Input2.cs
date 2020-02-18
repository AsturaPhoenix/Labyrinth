using UnityEngine;
using System.Collections;

public static class Input2
{
    // https://forum.unity.com/threads/mouse-sensitivity-in-webgl-way-too-sensitive.411574/
    // https://issuetracker.unity3d.com/issues/mouse-sensitivity-is-greater-in-webgl-than-in-the-editor
    public static float GetAxis(string axis)
    {
        return (Application.platform == RuntimePlatform.WebGLPlayer ? .5f : 1) * Input.GetAxis(axis);
    }
}
