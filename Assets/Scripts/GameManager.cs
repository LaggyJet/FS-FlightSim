using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {  get; private set; }
    [SerializeField] GameObject[] modes;
    [SerializeField] GameObject pauseMenu, background;
    public bool isPaused = false;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        EnableGameMode(Settings.GameMode.None); 
    }

    void Start() {
        EnableGameMode(Settings.Instance.selectedGameMode);
        ResumeGame();
    }

    void EnableGameMode(Settings.GameMode gameMode) {
        foreach (var mode in modes)
                mode.SetActive(gameMode.ToString() == mode.ToString().Split(' ')[0]);

    }

    public void PauseGame() {
        isPaused = true;
        background.SetActive(true);
        pauseMenu.SetActive(isPaused);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame() {
        isPaused = false;
        background.SetActive(false);
        pauseMenu.SetActive(isPaused);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
