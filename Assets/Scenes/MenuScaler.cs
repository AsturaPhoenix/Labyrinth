using UnityEngine;
using UnityEngine.UI;

public class MenuScaler : MonoBehaviour
{
    public float ReferenceDpi = 96;
    public Vector2 MinSize;

    private void Start()
    {
        int minDim = Mathf.Min(Screen.width, Screen.height);
        GetComponent<CanvasScaler>().scaleFactor = Mathf.Min(
            Screen.dpi / ReferenceDpi,
            minDim / MinSize.x,
            minDim / MinSize.y);
    }
}
