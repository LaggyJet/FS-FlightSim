using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {  get; private set; }
    public GameObject[] modes;
    public GameObject[] uiSettings; 
    [SerializeField] GameObject pauseMenu, background, winMenu, loseMenu, objectivesMenu;
    [SerializeField] TMP_Text loseCurTime, winTimeSpent, winAccAvg, winTimeAvg, winTotalAvg;
    public bool isPaused = false;
    public Tuple<GameObject, bool>[] objectivesCompleted;
    public float[] objectiveAccuries;
    public float accuracy = 0f;
    float overallTime = 0f;
    public bool startedGame = false;
    public bool finishedObjectives = false;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        EnableGameMode(Settings.GameMode.None); 
    }

    private void Update() {
        if (startedGame)
            overallTime += Time.deltaTime;
    }

    void Start() {
        EnableGameMode(Settings.Instance.selectedGameMode);
        ResumeGame();
    }

    void EnableGameMode(Settings.GameMode gameMode) {
        for (int i = 0; i < modes.Length; i++) {
            modes[i].SetActive(gameMode.ToString() == modes[i].name);
            uiSettings[i].SetActive(gameMode.ToString() == uiSettings[i].name);
        }
        if (gameMode == Settings.GameMode.FreeFlight)
            objectivesMenu.SetActive(false);
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

    public void CallWinGame() { StartCoroutine(WinGame()); }

    IEnumerator WinGame() {
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

    public void CallLoseGame() { StartCoroutine(LoseGame()); }

    IEnumerator LoseGame() {
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
