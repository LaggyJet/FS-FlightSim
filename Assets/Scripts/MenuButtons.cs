using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour {
    [SerializeField] GameObject mainMenu, optionsMenu, gameModesMenu;
    readonly List<GameObject> menus = new();

    void Awake() {
        if (mainMenu != null)
            menus.Add(mainMenu);
        if (optionsMenu != null)
            menus.Add(optionsMenu);
        if (gameModesMenu != null)
            menus.Add(gameModesMenu);
    }

    void EnableMenu(GameObject menu) { foreach (GameObject menu_ in menus) menu_.SetActive(menu == menu_); }

    public void PlayButton() { EnableMenu(gameModesMenu); }
    
    public void OptionsButton() { EnableMenu(optionsMenu); }

    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackButton() { EnableMenu(mainMenu); }

    public void TimeAttackButton() { }

    public void FreeFlightButton() { SceneManager.LoadScene("FreeFlight"); }

    public void ObstacleCourseButton() { }
}
