using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class UI : MonoBehaviour {
    static public UI Instance {  get; private set; }
    
    [Header("UI Variables")]
    [SerializeField] GameObject planeLevelTimer;
    [SerializeField] GameObject pauseBackground;
    [SerializeField] TMP_Text heliScorePlaceholder;
    [SerializeField] TMP_Text timerTime;
    [SerializeField] TMP_Text enemyKills;
    [SerializeField] TMP_Text friendlyKills;
    Transform gameModeUI;
    [Space]
    
    [Header("Button Variables")] 
    public bool canPause = true;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject firstGameModeButton;
    [SerializeField] GameObject firstSettingButton;
    [SerializeField] GameObject menuBackground;
    [SerializeField] GameObject planeRestartButton;
    [SerializeField] GameObject resumeButton;
    [Space]

    [Header("Main Title, Settings, GameModes")]
    public List<GameObject> titleMenus = new();
    [Space]

    [Header("Pause")]
    public List<GameObject> generalMenus = new();
    [Space]

    [Header("Plane Game Over")]
    public List<GameObject> planeMenus = new();
    GameObject warning;
    [Space]

    [Header("Heli Win, Heli Lose")]
    public List<GameObject> heliMenus = new();

    IEnumerable<GameObject> Menus {
        get {
            foreach (var menu in titleMenus) yield return menu;
            foreach (var menu in generalMenus) yield return menu;
            foreach (var menu in planeMenus) yield return menu;
            foreach (var menu in heliMenus) yield return menu;
        }
    }
    

    void Awake() {
        //singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //UI CODE


    public void EnterGameMode()
    {
        CloseMenus();
        planeLevelTimer.SetActive(true);
        gameModeUI = planeLevelTimer.transform.Find(GameManager.Instance.selectedGameMode.ToString());
        gameModeUI.gameObject.SetActive(true);
    }

    public void ExitGameMode()
    {
        GameManager.Instance.ResetVals();
        CloseMenus();
        gameModeUI.gameObject.SetActive(false);
        gameModeUI = null;
        planeLevelTimer.SetActive(false);
        GameManager.Instance.selectedGameMode = GameManager.GameMode.None;
    }

    public void ShowWarning(bool mode)
    {
        if(warning == null) warning = gameModeUI.transform.Find("Warning").gameObject;
        warning.SetActive(mode);
    }


    //MENUS AND BUTTONS CODE

    public void Lose() { canPause = false; planeLevelTimer.SetActive(false); pauseBackground.SetActive(true); EnableMenu(planeMenus[(int)PlaneMenuIndex.GAME_OVER]); EventSystem.current.SetSelectedGameObject(planeRestartButton); SetScores(); GameManager.Instance.Stop(); }
    public void Pause() { if (canPause) { pauseBackground.SetActive(true); planeLevelTimer.SetActive(false); EnableMenu(generalMenus[(int)GeneralMenuIndex.PAUSE]); EventSystem.current.SetSelectedGameObject(resumeButton); } }
    public void Win() { canPause = false; planeLevelTimer.SetActive(false); }
    public void EnableMenu(GameObject menu) { foreach (GameObject menu_ in Menus) menu_.SetActive(menu == menu_);  }
    public void CloseMenus() { foreach (GameObject menu_ in Menus) { menu_.SetActive(false); } menuBackground.SetActive(false); pauseBackground.SetActive(false);  }
    public void PlayButton() { EnableMenu(titleMenus[(int)TitleMenuIndex.GAME_MODES]); EventSystem.current.SetSelectedGameObject(firstGameModeButton); }
  
    public void SettingsButton() { EnableMenu(titleMenus[(int)TitleMenuIndex.SETTINGS]); EventSystem.current.SetSelectedGameObject(firstSettingButton); }

    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void UnPause() { if (canPause) { CloseMenus(); planeLevelTimer.SetActive(true); GameManager.Instance.Continue(); } }

    public void ReturnHome() { LoadMainMenu(); }

    public void BackButton() { EnableMenu(titleMenus[(int)TitleMenuIndex.MAIN_TITLE]); EventSystem.current.SetSelectedGameObject(playButton); }

    public void TimeAttackButton() { LoadGame("HeliMainScene", GameManager.GameMode.TimeAttack); CloseMenus(); }

    public void FreeFlightButton() { LoadGame("HeliMainScene", GameManager.GameMode.FreeFlight); CloseMenus(); }

    public void ObstacleCourseButton() { LoadGame("HeliMainScene", GameManager.GameMode.ObstacleCourse); CloseMenus(); }

    public void DogfightButton() { LoadGame("PlaneMainScene", GameManager.GameMode.DogFight); CloseMenus(); SetTimer(GameManager.Instance.timeMax); }

    void LoadMainMenu() {
        Time.timeScale = 1;
        canPause = true;
        ExitGameMode();
        EnableMenu(titleMenus[(int)TitleMenuIndex.MAIN_TITLE]);
        EventSystem.current.SetSelectedGameObject(playButton);
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

    public void SetTimer(float time) {
        int mins = (int)time / 60;
        int secs = (int)time % 60;

        if (mins <= 0 && secs <= 0)
            StartCoroutine(GameManager.Instance.currentManager.LoseGame());
        string minString;
        string secsString;

        if (mins < 10) minString = "0" + mins.ToString();
        else minString = mins.ToString();
        if(secs < 10) secsString = "0" + secs.ToString();
        else secsString = secs.ToString();

        string timeText = minString + ":" + secsString;

        timerTime.text = timeText;
    }









    public void PlaneGameOver() {
        canPause = false; 
        planeLevelTimer.SetActive(false); 
        pauseBackground.SetActive(true); 
        EnableMenu(planeMenus[(int)PlaneMenuIndex.GAME_OVER]); 
        EventSystem.current.SetSelectedGameObject(planeRestartButton); 
        SetScores(); 
        GameManager.Instance.Stop();
    }
}
