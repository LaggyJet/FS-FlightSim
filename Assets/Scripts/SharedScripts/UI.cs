using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using static UnityEngine.PlayerLoop.PreUpdate;
using TMPro;
using System.Collections;

public class UI : MonoBehaviour {
    //singleton code
    static public UI Instance {  get; private set; }
    [Header("UI Variables")]
    [SerializeField] GameObject gameModes;
    [SerializeField] GameObject pauseBackground;
    [SerializeField] TMP_Text heliScorePlaceholder, timerTime, enemyKills, friendlyKills;
    Transform gameModeUI;



    [Header("Menus and Buttons Variables")] 
    [HideInDebugUI] readonly List<GameObject> menus = new();
    public bool canPause = true;
    public GameObject mainMenu, settingsMenu, gameModesMenu, loseMenu, pauseMenu, winMenu;
    [SerializeField] GameObject mainButton, gameModesButton, settingsButton, menuBackground, loseButton, resumeButton;
    GameObject warning;
    

    void Awake() {
        //singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        //adds menus to the list
        if (mainMenu != null)
            menus.Add(mainMenu);
        if (settingsMenu != null)
            menus.Add(settingsMenu);
        if (gameModesMenu != null)
            menus.Add(gameModesMenu);
        if (loseMenu != null)
            menus.Add(loseMenu);
        if (winMenu != null)
            menus.Add(winMenu);
        if (pauseMenu != null)
            menus.Add(pauseMenu);
    }

    //UI CODE


    public void EnterGameMode()
    {
        CloseMenus();
        gameModes.SetActive(true);
        gameModeUI = gameModes.transform.Find(GameManager.Instance.selectedGameMode.ToString());
        gameModeUI.gameObject.SetActive(true);
    }

    public void ExitGameMode()
    {
        GameManager.Instance.ResetVals();
        CloseMenus();
        gameModeUI.gameObject.SetActive(false);
        gameModeUI = null;
        gameModes.SetActive(false);
        GameManager.Instance.selectedGameMode = GameManager.GameMode.None;
    }

    public void ShowWarning(bool mode)
    {
        if(warning == null) warning = gameModeUI.transform.Find("Warning").gameObject;
        warning.SetActive(mode);
    }


    //MENUS AND BUTTONS CODE

    public void Lose() { canPause = false; gameModes.SetActive(false); pauseBackground.SetActive(true); EnableMenu(loseMenu); EventSystem.current.SetSelectedGameObject(loseButton); SetScores(); GameManager.Instance.Stop(); }
    public void Pause() { if (canPause) { pauseBackground.SetActive(true); gameModes.SetActive(false); EnableMenu(pauseMenu); EventSystem.current.SetSelectedGameObject(resumeButton); } }
    public void Win() { canPause = false; gameModes.SetActive(false); EnableMenu(winMenu); }
    public void EnableMenu(GameObject menu) { foreach (GameObject menu_ in menus) menu_.SetActive(menu == menu_);  }
    public void CloseMenus() { foreach (GameObject menu_ in menus) { menu_.gameObject.SetActive(false); } menuBackground.SetActive(false); pauseBackground.SetActive(false);  }
    public void PlayButton() { EnableMenu(gameModesMenu); EventSystem.current.SetSelectedGameObject(gameModesButton); }
  
    public void SettingsButton() { EnableMenu(settingsMenu); EventSystem.current.SetSelectedGameObject(settingsButton); }

    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void UnPause() { if (canPause) { CloseMenus(); gameModes.SetActive(true); GameManager.Instance.Continue(); } }

    public void ReturnHome() { LoadMainMenu(); }

    public void BackButton() { EnableMenu(mainMenu); EventSystem.current.SetSelectedGameObject(mainButton); }

    public void TimeAttackButton() { LoadGame("HeliMainScene", GameManager.GameMode.TimeAttack); CloseMenus(); }

    public void FreeFlightButton() { LoadGame("HeliMainScene", GameManager.GameMode.FreeFlight); CloseMenus(); }

    public void ObstacleCourseButton() { LoadGame("HeliMainScene", GameManager.GameMode.ObstacleCourse); CloseMenus(); }

    public void DogfightButton() { LoadGame("PlaneMainScene", GameManager.GameMode.DogFight); CloseMenus(); SetTimer(GameManager.Instance.timeMax); }

    void LoadMainMenu() {
        Time.timeScale = 1;
        canPause = true;
        ExitGameMode();
        EnableMenu(mainMenu);
        EventSystem.current.SetSelectedGameObject(mainButton);
        menuBackground.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu");
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Menu);
    }

    void LoadGame( string scene, GameManager.GameMode mode) {
        SceneManager.LoadScene(scene);
        GameManager.Instance.selectedGameMode = mode;
        GameManager.Instance.ResumeGame();
        EnterGameMode();
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Level);
    }



    public void RestartButton() {
        UnPause();
        canPause = true ;
        if (GameManager.Instance.currentManager is PlaneGameManager planeManager)
            planeManager.ResetVals();
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
            case GameManager.GameMode.DogFight:
                DogfightButton();
                break;
        }
    }

    public void SetScores()
    {
        switch (GameManager.Instance.selectedGameMode)
        {
            case GameManager.GameMode.TimeAttack:
                break;
            case GameManager.GameMode.FreeFlight:
                break;
            case GameManager.GameMode.DogFight:
                {
                    if (GameManager.Instance.currentManager is PlaneGameManager planeManager) {
                        enemyKills.text = planeManager.enemiesKilled.ToString();
                        friendlyKills.text = planeManager.friendliesKilled.ToString();
                    }
                    break;
                }
        }
    }

    public void SetTimer(float time)
    {
        int mins = (int)time / 60;
        int secs = (int)time % 60;

        if (mins <= 0 && secs <= 0) Lose();

        string minString = string.Empty;
        string secsString = string.Empty;

        if (mins < 10) minString = "0" + mins.ToString();
        else minString = mins.ToString();
        if(secs < 10) secsString = "0" + secs.ToString();
        else secsString = secs.ToString();

        string timeText = minString + ":" + secsString;

        timerTime.text = timeText;
    }
}
