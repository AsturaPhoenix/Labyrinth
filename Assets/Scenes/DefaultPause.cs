using UnityEngine;

public class DefaultPause : MonoBehaviour, Pause
{
    public void Pause()
    {
        foreach (var e in GetComponentsInChildren<MonoBehaviour>())
            e.enabled = false;
    }

    public void Resume()
    {
        foreach (var e in GetComponentsInChildren<MonoBehaviour>())
            e.enabled = true;
    }
}
