using UnityEngine;
using System.Collections;

public class EscapeMenu : MonoBehaviour
{
    private GameObject instance;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !instance)
        {
            instance = Instantiate(Resources.Load<GameObject>("Game Over Menu"), transform);
            instance.GetComponent<GameOverMenu>().Parent = gameObject;
        }
    }
}
