using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public abstract class GameManager : MonoBehaviour {
    //singleton
    [HideInInspector] public static GameManager Instance { get; private set; }
    //game mode enum
    [HideInInspector] public enum GameMode { None, TimeAttack, FreeFlight, ObstacleCourse, DogFight, LandingTrial };

    [HideInInspector] public enum ManagerTypes { Heli, Plane };

    [Tooltip("Running Variables")]
    public bool isPaused = false;


    [Header("Game Important Variables")]
    public GameMode selectedGameMode = GameMode.None;
    public GameManager currentManager;
    [SerializeField] public GameObject pauseMenu, background, winMenu, loseMenu, objectivesMenu, pauseButton, winButton, loseButton;
    public GameObject[] modes;
    public GameObject[] uiSettings;
    [SerializeField] public float timeMax = 300f;

    void SetGameManager(ManagerTypes type) {
        switch (type) {
            case ManagerTypes.Heli:
                currentManager = FindFirstObjectByType<HeliGameManager>();
                break;
            case ManagerTypes.Plane:
                currentManager = FindFirstObjectByType<PlaneGameManager>();
                break;
            default:
                break;
        }
    }




    void Awake()
    {
        //singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        EnableGameMode(GameMode.None);
        EventSystem.current.SetSelectedGameObject(pauseButton);
    }

    void Update() { currentManager?.Update(); }

    void Start()
    {
        EnableGameMode(selectedGameMode);
        ResumeGame();
    }

    void EnableGameMode(GameMode gameMode)
    {
        for (int i = 0; i < modes.Length; i++)
        {
            modes[i].SetActive(gameMode.ToString() == modes[i].name);
            uiSettings[i].SetActive(gameMode.ToString() == uiSettings[i].name);
        }
        if (gameMode == GameMode.FreeFlight)
            objectivesMenu.SetActive(false);
    }

    public void OnPauseResume() { print("paused"); (Instance.isPaused ? (Action)Instance.ResumeGame : Instance.PauseGame)(); }

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

    public abstract void ResetVals();

    public void CallWinGame() { StartCoroutine(currentManager?.WinGame()); }

    public abstract IEnumerator WinGame();

    public void CallLoseGame() { StartCoroutine(currentManager?.LoseGame()); }

    public abstract IEnumerator LoseGame();
}
