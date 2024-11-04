using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    [SerializeField] GameObject mainMenu, settingsMenu, gameModesMenu;
    readonly List<GameObject> menus = new();

    void Awake() {
        if (mainMenu != null)
            menus.Add(mainMenu);
        if (settingsMenu != null)
            menus.Add(settingsMenu);
        if (gameModesMenu != null)
            menus.Add(gameModesMenu);
    }

    void EnableMenu(GameObject menu) { foreach (GameObject menu_ in menus) menu_.SetActive(menu == menu_); }

    public void PlayButton() { EnableMenu(gameModesMenu); }
    
    public void SettingsButton() { EnableMenu(settingsMenu); }

    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ContinueGame() { GameManager.Instance.ResumeGame(); }

    public void ReturnHome() { SceneManager.LoadScene("MainMenu"); }

    public void BackButton() { EnableMenu(mainMenu); }

    public void TimeAttackButton() { SceneManager.LoadScene("MainScene"); Settings.Instance.selectedGameMode = Settings.GameMode.TimeAttack; }

    public void FreeFlightButton() { SceneManager.LoadScene("MainScene"); Settings.Instance.selectedGameMode = Settings.GameMode.FreeFlight; }

    public void ObstacleCourseButton() { SceneManager.LoadScene("MainScene"); Settings.Instance.selectedGameMode = Settings.GameMode.ObstacleCourse; }
}
