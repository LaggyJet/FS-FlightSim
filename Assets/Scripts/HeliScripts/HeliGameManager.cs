using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class HeliGameManager : GameManager {
    [Header("Heli Stuff")]
    [SerializeField] TMP_Text loseCurTime, winTimeSpent, winAccAvg, winTimeAvg, winTotalAvg;
    public Tuple<GameObject, bool>[] objectivesCompleted;
    public float[] objectiveAccuries;
    public float accuracy = 0f;
    float overallTime = 0f;
    public bool startedGame = false;
    public bool finishedObjectives = false;

    void Update() {
        if (startedGame)
            overallTime += Time.deltaTime;
    }

    public override IEnumerator WinGame() {
        EventSystem.current.SetSelectedGameObject(UI.Instance.heliWinRestartButton);
        startedGame = false;
        yield return new WaitForSeconds(3f);
        winTimeSpent.text = TimeSpan.FromSeconds(overallTime).ToString(@"hh\:mm\:ss\:fff");
        winAccAvg.text = accuracy.ToString("F2");
        float timeScore = ScoreChecker.GetTimeRank(overallTime);
        winTimeAvg.text = timeScore.ToString("F2");
        winTotalAvg.text = ((timeScore + accuracy) / 2).ToString("F2");
        UI.Instance.pauseBackground.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        UI.Instance.gameModeUI.gameObject.SetActive(false);
        UI.Instance.heliMenus[(int)HeliMenuIndex.WIN].SetActive(true);
    }

    public override IEnumerator LoseGame() {
        EventSystem.current.SetSelectedGameObject(UI.Instance.heliLoseRestartButton);
        startedGame = false;
        yield return new WaitForSeconds(1f);
        loseCurTime.text = TimeSpan.FromSeconds(overallTime).ToString(@"hh\:mm\:ss\:fff");
        UI.Instance.pauseBackground.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        UI.Instance.gameModeUI.gameObject.SetActive(false);
        UI.Instance.heliMenus[(int)HeliMenuIndex.LOSE].SetActive(true);
    }

    public override void ResetVals() {

    }
}