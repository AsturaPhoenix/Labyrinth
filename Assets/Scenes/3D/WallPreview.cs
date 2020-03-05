using UnityEngine;

public class WallPreview : MonoBehaviour
{
    public GameObject Wall, Edge, Corner;
    public bool IncludeCorner;

    private GameObject InstantiateChild(GameObject template)
    {
        var obj = Instantiate(template, transform);
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            child.gameObject.layer = gameObject.layer;
        return obj;
    }
    
    private void Start()
    {
        if (Wall != null)
            InstantiateChild(Wall);

        if (Edge != null)
        {
            InstantiateChild(Edge).transform.localPosition = new Vector3(0, -.5f);
            InstantiateChild(Edge).transform.localPosition = new Vector3(0, .5f);
            var edge = InstantiateChild(Edge).transform;
            edge.localPosition = new Vector3(-.5f, 0);
            edge.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            edge = InstantiateChild(Edge).transform;
            edge.localPosition = new Vector3(.5f, 0);
            edge.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        }

        if (Corner != null)
        {
            InstantiateChild(Corner).transform.localPosition = new Vector3(-.5f, -.5f);
            InstantiateChild(Corner).transform.localPosition = new Vector3(.5f, -.5f);
            InstantiateChild(Corner).transform.localPosition = new Vector3(-.5f, .5f);
            InstantiateChild(Corner).transform.localPosition = new Vector3(.5f, .5f);
        }
    }
}
