using UnityEngine;

public class EscapeMenu : MonoBehaviour, Pause
{
    public void Pause() => enabled = false;
    public void Resume() => enabled = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameOverMenu.Instantiate(gameObject);
    }
}
