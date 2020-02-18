using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject Parent;

    public Game Game => Parent.GetComponent<Game>();

    private float timeScale;
    private CursorLockMode cursorLockMode;

    public void NewMaze()
    {
        Game.NewMaze();
        Destroy();
    }

    public void Settings()
    {
        Game.Settings();
        Destroy();
    }

    public void Export() => Game.Export();

    public void Import()
    {
        if (Game.Import())
            Destroy();
    }

    public void MainMenu() => SceneManager.LoadScene("Main Menu");

    private void Start()
    {
        cursorLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;

        timeScale = Time.timeScale;
        Time.timeScale = 0;

        foreach (var pausable in Parent.GetComponentsInChildren<Pause>())
            pausable.Pause();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);

        // These cleanups only apply if we're resuming.
        Cursor.lockState = cursorLockMode;

        foreach (var pausable in Parent.GetComponentsInChildren<Pause>())
            pausable.Resume();
    }

    private void OnDestroy()
    {
        // These cleanups apply regardless of whether we're resuming or going back to the main menu.
        Time.timeScale = timeScale;
    }
}
