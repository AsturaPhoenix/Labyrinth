using UnityEngine;

public class GameMenuLauncher : MonoBehaviour, Pause
{
    public GameObject Menu;

    public void Pause() => enabled = false;
    public void Resume() => enabled = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Show();
    }

    public GameObject Show()
    {
        var instance = Instantiate(Menu);
        instance.GetComponentInChildren<GameMenu>().GameRoot = gameObject;
        return instance;
    }
}
