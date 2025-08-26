using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    //debug
    [SerializeField, HideInDebugUI] bool debug;
    //singleton
    [HideInInspector] public static GameManager Instance { get; private set; }
    //game mode enum
    [HideInInspector] public enum GameMode { None, TimeAttack, FreeFlight, ObstacleCourse, DogFight, LandingTrial };

    [Tooltip("Running Variables")]
    public bool isPaused = false;


    [Header("Game Important Variables")]
    public GameMode selectedGameMode = GameMode.None;
    [Tooltip("DogFight Variables")]
    public int enemies = 0;
    public int enemiesKilled = 0;
    [SerializeField] public int enemiesMax = 100;
    public int friendlies = 0;
    public int friendliesKilled = 0;
    [SerializeField] public int friendliesMax = 100;
    public bool runTimer = false;
    public float timeNow = 0f;
    [SerializeField] public float timeMax = 300f;

    [Header("Heli Stuff")]
    public GameObject[] modes;
    public GameObject[] uiSettings;
    [SerializeField] GameObject pauseMenu, background, winMenu, loseMenu, objectivesMenu, pauseButton, winButton, loseButton;
    [SerializeField] TMP_Text loseCurTime, winTimeSpent, winAccAvg, winTimeAvg, winTotalAvg;
    public Tuple<GameObject, bool>[] objectivesCompleted;
    public float[] objectiveAccuries;
    public float accuracy = 0f;
    float overallTime = 0f;
    public bool startedGame = false;
    public bool finishedObjectives = false;


    void Awake()
    {
        //singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        EnableGameMode(Settings.GameMode.None);
        EventSystem.current.SetSelectedGameObject(pauseButton);
    }

    private void Update()
    {
        if (debug)
        {
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick button " + i))
                {
                    Debug.Log("Button " + i + " was pressed!");
                }
            }
        }
        if (runTimer) { timeNow += Time.deltaTime; UI.Instance.SetTimer(timeMax - timeNow); }
        if (Input.GetButtonDown("Cancel") && selectedGameMode != GameMode.None) OnPauseResume();


        //heli
        if (startedGame)
            overallTime += Time.deltaTime;
    }

    void Start()
    {
        EnableGameMode(Settings.Instance.selectedGameMode);
        ResumeGame();
    }

    void EnableGameMode(Settings.GameMode gameMode)
    {
        for (int i = 0; i < modes.Length; i++)
        {
            modes[i].SetActive(gameMode.ToString() == modes[i].name);
            uiSettings[i].SetActive(gameMode.ToString() == uiSettings[i].name);
        }
        if (gameMode == Settings.GameMode.FreeFlight)
            objectivesMenu.SetActive(false);
    }

    void OnPauseResume() { print("paused"); (GameManager.Instance.isPaused ? (Action)GameManager.Instance.ResumeGame : GameManager.Instance.PauseGame)(); }

    public void PauseGame()
    {
        if (selectedGameMode == GameMode.None) return;
        UI.Instance.Pause();
        Stop();


        isPaused = true;
        background.SetActive(true);
        pauseMenu.SetActive(isPaused);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Stop()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        UI.Instance.UnPause();

        MenuButtons.Instance.settingsMenu.SetActive(false);
        isPaused = false;
        background.SetActive(false);
        pauseMenu.SetActive(isPaused);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Continue()
    {
        UI.Instance.settingsMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CallWinGame() { StartCoroutine(WinGame()); }

    IEnumerator WinGame()
    {
        //EventSystem.current.SetSelectedGameObject(UIUpdater.Instance.winButton);
        yield return new WaitForSeconds(3f);
        //UIUpdater.Instance.background.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        //UIUpdater.Instance.winMenu.SetActive(true);

        //update the scores




        EventSystem.current.SetSelectedGameObject(winButton);
        startedGame = false;
        yield return new WaitForSeconds(3f);
        winTimeSpent.text = TimeSpan.FromSeconds(overallTime).ToString(@"hh\:mm\:ss\:fff");
        winAccAvg.text = accuracy.ToString("F2");
        float timeScore = ScoreChecker.GetTimeRank(overallTime);
        winTimeAvg.text = timeScore.ToString("F2");
        winTotalAvg.text = ((timeScore + accuracy) / 2).ToString("F2");
        background.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        objectivesMenu.SetActive(false);
        winMenu.SetActive(true);
    }

    public void ResetVals()
    {
        timeNow = 0;
        runTimer = false;
        enemies = 0;
        friendlies = 0;
        enemiesKilled = 0;
        friendliesKilled = 0;
    }

    public void CallLoseGame() { StartCoroutine(LoseGame()); }

    IEnumerator LoseGame()
    {
        EventSystem.current.SetSelectedGameObject(loseButton);
        startedGame = false;
        yield return new WaitForSeconds(1f);
        loseCurTime.text = TimeSpan.FromSeconds(overallTime).ToString(@"hh\:mm\:ss\:fff");
        background.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        objectivesMenu.SetActive(false);
        loseMenu.SetActive(true);
    }
}
