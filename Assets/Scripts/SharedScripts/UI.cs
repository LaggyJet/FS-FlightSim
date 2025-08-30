using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System;

public class UI : MonoBehaviour {
    static public UI Instance {  get; private set; }
    
    [Header("UI Variables")]
    [SerializeField] GameObject planeLevelTimer;
    public GameObject pauseBackground;
    [SerializeField] TMP_Text heliScorePlaceholder;
    [SerializeField] TMP_Text timerTime;
    [SerializeField] TMP_Text enemyKills;
    [SerializeField] TMP_Text friendlyKills;
    [HideInInspector] public Transform gameModeUI;
    [Space]
    
    [Header("Button Variables")] 
    public bool canPause = true;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject firstGameModeButton;
    [SerializeField] GameObject firstSettingButton;
    [SerializeField] GameObject menuBackground;
    [SerializeField] GameObject planeRestartButton;
    [SerializeField] GameObject resumeButton;
    public GameObject heliWinRestartButton;
    public GameObject heliLoseRestartButton;
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
    [Space]

    [Header("Heli Objectives")]
    public GameObject[] heliObjectives;
    [Space]

    [Header("Heli Launcher Things")]
    public GameObject warningTextContainer;
    public TMP_Text warningText, countdownText;
    [Space]

    [Header("Heli Win/Lose Info Params")]
    [SerializeField] TMP_Text loseCurTime;
    [SerializeField] TMP_Text winTimeSpent;
    [SerializeField] TMP_Text winAccAvg;
    [SerializeField] TMP_Text winTimeAvg;
    [SerializeField] TMP_Text winTotalAvg;

    IEnumerable<GameObject> Menus => new[] { titleMenus, generalMenus, planeMenus, heliMenus }.SelectMany(m => m);
    

    void Awake() {
        //Singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //Title Menu Buttons
    public void PlayButton() { 
        EnableMenu(titleMenus[(int)TitleMenuIndex.GAME_MODES]); 
        EventSystem.current.SetSelectedGameObject(firstGameModeButton); 
    }

    public void SettingsButton() { 
        EnableMenu(titleMenus[(int)TitleMenuIndex.SETTINGS]); 
        EventSystem.current.SetSelectedGameObject(firstSettingButton); 
    }
    
    public void ExitButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    //Plane Game Mode Buttons
    public void DogfightButton() {
        LoadGame("PlaneMainScene", GameMode.DogFight);
        CloseMenus();
        if (GameManager.Instance.currentModeManager is PlaneGameManager planeManager)
            SetTimer(planeManager.timeMax);
    }

    //Heli Game Mode Buttons
    public void TimeAttackButton() { 
        LoadGame("HeliMainScene", GameMode.TimeAttack); 
        CloseMenus(); 
    }

    public void FreeFlightButton() {
        LoadGame("HeliMainScene", GameMode.FreeFlight); 
        CloseMenus(); 
    }

    public void ObstacleCourseButton() { 
        LoadGame("HeliMainScene", GameMode.ObstacleCourse); 
        CloseMenus(); 
    }

    //Helper UI Functions
    void LoadGame(string scene, GameMode mode) {
        Time.timeScale = 1;
        GameManager.Instance.SetGameManager(mode);
        SceneManager.LoadScene(scene);
        GameManager.Instance.EnableGameMode(mode);
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Level);


        //GameManager.Instance.ResumeGame();




        if (GameManager.Instance.currentModeManager is PlaneGameManager)
            EnterGameMode();
    }

    public void EnterGameMode() {
        planeLevelTimer.SetActive(true);
        gameModeUI = planeLevelTimer.transform.Find(GameManager.Instance.selectedGameMode.ToString());
        gameModeUI.gameObject.SetActive(true);
    }

    public void ExitGameMode() {
        GameManager.Instance.currentModeManager.ResetVals();
        gameModeUI.gameObject.SetActive(false);
        gameModeUI = null;
        planeLevelTimer.SetActive(false);
        GameManager.Instance.selectedGameMode = GameMode.None;
    }

    public void EnableMenu(GameObject menu) { 
        foreach (GameObject menu_ in Menus) 
            menu_.SetActive(menu == menu_); 
    }
    
    public void CloseMenus() { 
        foreach (GameObject menu_ in Menus) 
            menu_.SetActive(false);  
        menuBackground.SetActive(false); 
        pauseBackground.SetActive(false); 
    }

    //UI Updater Functions
    public void ShowWarning(bool mode) {
        if(warning == null) 
            warning = gameModeUI.transform.Find("Warning").gameObject;
        warning.SetActive(mode);
    }

    public void SetHeliObjectives() {
        if (GameManager.Instance.currentModeManager is HeliGameManager heliManager) {
            heliManager.uiSettings = heliObjectives;
            if (GameManager.Instance.selectedGameMode.mode != (int)GameMode.HeliMode.FreeFlight)
                heliManager.uiSettings[0].transform.parent.gameObject.SetActive(true);
            List<Transform> tfs = new();
            switch (GameManager.Instance.selectedGameMode.category) {
                case GameMode.Category.Heli:
                    switch ((GameMode.HeliMode)GameManager.Instance.selectedGameMode.mode) {
                        case GameMode.HeliMode.TimeAttack:
                            heliManager.curObjectiveObject = heliManager.uiSettings[0];
                            // TODO: make sure to update find when adding TimeAttack
                            heliManager.curObjectiveText = heliManager.curObjectiveObject.transform.Find("").GetComponent<TMP_Text>();
                            heliManager.curObjectiveMax = heliManager.modes[0].transform.childCount;
                            for (int i = 0; i < heliManager.curObjectiveMax; i++)
                                tfs.Add(heliManager.modes[0].transform.GetChild(i));
                            break;

                        case GameMode.HeliMode.ObstacleCourse:
                            heliManager.curObjectiveObject = heliManager.uiSettings[1];
                            heliManager.curObjectiveText = heliManager.curObjectiveObject.transform.Find("LandingZones/Completed").GetComponent<TMP_Text>();
                            heliManager.curObjectiveMax = heliManager.modes[1].transform.childCount;
                            for (int i = 0; i < heliManager.curObjectiveMax; i++)
                                tfs.Add(heliManager.modes[1].transform.GetChild(i));
                            break;
                    }
                    heliManager.objectivesCompleted = new System.Tuple<GameObject, bool>[heliManager.curObjectiveMax];
                    heliManager.objectiveAccuries = new float[heliManager.curObjectiveMax];
                    for (int i = 0; i < tfs?.Count; i++)
                        heliManager.objectivesCompleted[i] = Tuple.Create(tfs[i].gameObject, false);
                    break;
            }
        }
    }

    public void UpdateCurrentHeliObjectiveScore(int newScore = int.MinValue) {
        if (GameManager.Instance.currentModeManager is HeliGameManager heliManager) {
            if (!int.TryParse(heliManager.curObjectiveText.text, out int score))
                score = -1;
            heliManager.curScore = score + (newScore == int.MinValue ? 1 : newScore);
            heliManager.curObjectiveText.text = heliManager.curScore.ToString();
            if (heliManager.curScore >= heliManager.curObjectiveMax)
                heliManager.finishedObjectives = true;
        }
    }

    public void SetHeliUI() {
        if (GameManager.Instance.currentModeManager is HeliGameManager heliManager) {
            heliManager.loseCurTime = loseCurTime;
            heliManager.winTimeSpent = winTimeSpent;
            heliManager.winAccAvg = winAccAvg;
            heliManager.winTimeAvg = winTimeAvg;
            heliManager.winTotalAvg = winTotalAvg;
        }
    }

    public void SetPlaneUI() {

    }

















    public void Lose() { canPause = false; planeLevelTimer.SetActive(false); pauseBackground.SetActive(true); EnableMenu(planeMenus[(int)PlaneMenuIndex.GAME_OVER]); EventSystem.current.SetSelectedGameObject(planeRestartButton); SetScores(); GameManager.Instance.Stop(); }
    public void Pause() { if (canPause) { pauseBackground.SetActive(true); planeLevelTimer.SetActive(false); EnableMenu(generalMenus[(int)GeneralMenuIndex.PAUSE]); EventSystem.current.SetSelectedGameObject(resumeButton); } }
    public void Win() { canPause = false; planeLevelTimer.SetActive(false); }
    
  


    public void UnPause() { 
        if (canPause) { 
            CloseMenus(); 
            //planeLevelTimer.SetActive(true);
            GameManager.Instance.Continue(); 
        } 
    }

    public void ReturnHome() { LoadMainMenu(); }

    public void BackButton() { EnableMenu(titleMenus[(int)TitleMenuIndex.MAIN_TITLE]); EventSystem.current.SetSelectedGameObject(playButton); }

    

    

    void LoadMainMenu() {
        Time.timeScale = 1;
        canPause = true;
        //ExitGameMode();
        pauseBackground.SetActive(false);
        EnableMenu(titleMenus[(int)TitleMenuIndex.MAIN_TITLE]);
        EventSystem.current.SetSelectedGameObject(playButton);
        menuBackground.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu");
        AudioController.Instance.FadeAudio(3f, AudioController.BackgroundTypes.Menu);
    }

    



    public void RestartButton() {
        UnPause();
        canPause = true;
        if (GameManager.Instance.currentModeManager is PlaneGameManager planeManager)
            planeManager.ResetVals();
        switch (GameManager.Instance.selectedGameMode.category) {
            case GameMode.Category.Heli:
                switch ((GameMode.HeliMode)GameManager.Instance.selectedGameMode.mode) {
                    case GameMode.HeliMode.TimeAttack:
                        TimeAttackButton();
                        break;
                    case GameMode.HeliMode.FreeFlight:
                        FreeFlightButton();
                        break;
                    case GameMode.HeliMode.ObstacleCourse:
                        ObstacleCourseButton();
                        break;
                }
                break;

            case GameMode.Category.Plane:
                switch ((GameMode.PlaneMode)GameManager.Instance.selectedGameMode.mode) {
                    case GameMode.PlaneMode.DogFight:
                        DogfightButton();
                        break;
                }
                break;
        }
    }

    public void SetScores() {
        switch (GameManager.Instance.selectedGameMode.category) {
            case GameMode.Category.Plane:
                switch ((GameMode.PlaneMode)GameManager.Instance.selectedGameMode.mode) {
                    case GameMode.PlaneMode.DogFight:
                        if (GameManager.Instance.currentModeManager is PlaneGameManager planeManager) {
                            enemyKills.text = planeManager.enemiesKilled.ToString();
                            friendlyKills.text = planeManager.friendliesKilled.ToString();
                        }
                        break;
                }
                break;
        }
    }

    public void SetTimer(float time) {
        int mins = (int)time / 60;
        int secs = (int)time % 60;

        if (mins <= 0 && secs <= 0)
            StartCoroutine(GameManager.Instance.currentModeManager.LoseGame());
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
