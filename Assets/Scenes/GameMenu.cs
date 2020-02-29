using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour, MenuStack
{
    public GameObject GameRoot, Carousel, RootMenu, ExportMenu, ImportMenu;

    public Game Game => GameRoot.GetComponent<Game>();

    private float timeScale;
    private CursorLockMode cursorLockMode;
    private Stack<GameObject> stack = new Stack<GameObject>();

    public void NewMaze()
    {
        Game.NewMaze();
        Destroy();
    }

    public void Settings()
    {
        Shift(Game.Settings());
    }

    public void Export()
    {
        var menuInstance = Instantiate(ExportMenu);
        // TODO: If the character count exceeds 16k or so, we'll run into a mesh vertex limit.
        menuInstance.GetComponent<Export>().Serialized.text = Game.Export();
        Shift(menuInstance);
    }

    public void Import()
    {
        var menuInstance = Instantiate(ImportMenu);
        menuInstance.GetComponent<Import>().Game = Game;
        Shift(menuInstance);
    }

    public void MainMenu() => SceneManager.LoadScene("Main Menu");

    public void Shift(GameObject submenu)
    {
        stack.Peek().SetActive(false);

        submenu.GetComponent<Submenu>().Menu = this;
        submenu.transform.SetParent(Carousel.transform, false);
        
        stack.Push(submenu);
    }

    public void Pop()
    {
        Debug.Assert(stack.Count > 1);

        Destroy(stack.Pop());
        stack.Peek().SetActive(true);
    }

    public void Destroy()
    {
        Destroy(gameObject);

        // These cleanups only apply if we're resuming.
        Cursor.lockState = cursorLockMode;

        foreach (var pausable in GameRoot.GetComponentsInChildren<Pause>())
            pausable.Resume();
    }

    private void Start()
    {
        cursorLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;

        timeScale = Time.timeScale;
        Time.timeScale = 0;

        foreach (var pausable in GameRoot.GetComponentsInChildren<Pause>())
            pausable.Pause();

        stack.Push(RootMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Destroy();
    }

    private void OnDestroy()
    {
        // These cleanups apply regardless of whether we're resuming or going back to the main menu.
        Time.timeScale = timeScale;
    }
}
