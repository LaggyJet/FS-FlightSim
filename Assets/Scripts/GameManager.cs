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
        switch (Settings.Instance.selectedGameMode) {
            case Settings.GameMode.TimeAttack:
                EnableGameMode(Settings.GameMode.TimeAttack);
                break;
            case Settings.GameMode.FreeFlight:
                EnableGameMode(Settings.GameMode.FreeFlight);
                break;
            case Settings.GameMode.ObstacleCourse:
                EnableGameMode(Settings.GameMode.ObstacleCourse);
                break;
            default:
                break;
        }
        ResumeGame();
    }

    void EnableGameMode(Settings.GameMode gameMode) {
        foreach (var mode in modes) {
            if (gameMode.ToString() == mode.ToString().Split(' ')[0])
                mode.SetActive(true);
            else
                mode.SetActive(false);
        }
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
