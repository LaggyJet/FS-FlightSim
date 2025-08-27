using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuButtons : MonoBehaviour {
    static public MenuButtons Instance {  get; private set; }
    public GameObject mainMenu, settingsMenu, gameModesMenu;
    [SerializeField] GameObject mainButton, gameModesButton, settingsButton;
    readonly List<GameObject> menus = new();

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        if (mainMenu != null)
            menus.Add(mainMenu);
        if (settingsMenu != null)
            menus.Add(settingsMenu);
        if (gameModesMenu != null)
            menus.Add(gameModesMenu);
    }

    public void EnableMenu(GameObject menu) { foreach (GameObject menu_ in menus) menu_.SetActive(menu == menu_); }

    public void PlayButton() { EnableMenu(gameModesMenu); EventSystem.current.SetSelectedGameObject(gameModesButton); }
    
    public void SettingsButton() { EnableMenu(settingsMenu); EventSystem.current.SetSelectedGameObject(settingsButton); }

    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ContinueGame() { GameManager.Instance.ResumeGame(); }

    public void ReturnHome() { LoadMainScene(); EventSystem.current.SetSelectedGameObject(mainButton); }

    public void BackButton() { EnableMenu(mainMenu); EventSystem.current.SetSelectedGameObject(mainButton); }

    public void TimeAttackButton() { LoadGame(); GameManager.Instance.selectedGameMode = GameManager.GameMode.TimeAttack; }

    public void FreeFlightButton() { LoadGame(); GameManager.Instance.selectedGameMode = GameManager.GameMode.FreeFlight; }

    public void ObstacleCourseButton() { LoadGame(); GameManager.Instance.selectedGameMode = GameManager.GameMode.ObstacleCourse; }

    void LoadMainScene() {
        SceneManager.LoadScene("MainMenu");
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Menu);
    }

    void LoadGame() {
        SceneManager.LoadScene("MainScene");
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Level);
    }

    public void RestartButton() {
        switch (GameManager.Instance.selectedGameMode) {
            case GameManager.GameMode.TimeAttack:
                TimeAttackButton();
                break;
            case GameManager.GameMode.FreeFlight:
                FreeFlightButton();
                break;
            case GameManager.GameMode.ObstacleCourse:
                ObstacleCourseButton();
                break;
        }
    }
}
