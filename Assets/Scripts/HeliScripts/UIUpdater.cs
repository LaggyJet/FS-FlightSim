using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour {
    public static UIUpdater Instance { get; private set; }
    GameObject curObjectiveObject;
    TMP_Text curObjectiveText;
    public int curObjectiveMax, curScore = 0;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start() {
        List<Transform> tfs = new();
        switch (Settings.Instance.selectedGameMode) {
            case Settings.GameMode.TimeAttack:
                curObjectiveObject = GameManager.Instance.uiSettings[0];
                //TODO: make sure to update find when adding time attack
                curObjectiveText = curObjectiveObject.transform.Find("").GetComponent<TMP_Text>();
                curObjectiveMax = GameManager.Instance.modes[0].transform.childCount;
                for (int i = 0; i < curObjectiveMax; i++)
                    tfs.Add(GameManager.Instance.modes[0].transform.GetChild(i));
                break;
            case Settings.GameMode.ObstacleCourse:
                curObjectiveObject = GameManager.Instance.uiSettings[1];
                curObjectiveText = curObjectiveObject.transform.Find("LandingZones/Completed").GetComponent<TMP_Text>();
                curObjectiveMax = GameManager.Instance.modes[1].transform.childCount;
                for (int i = 0; i < curObjectiveMax; i++)
                    tfs.Add(GameManager.Instance.modes[1].transform.GetChild(i));
                break;
        }
        GameManager.Instance.objectivesCompleted = new System.Tuple<GameObject, bool>[curObjectiveMax];
        GameManager.Instance.objectiveAccuries = new float[curObjectiveMax];
        for (int i = 0; i < tfs?.Count; i++)
                GameManager.Instance.objectivesCompleted[i] = Tuple.Create(tfs[i].gameObject, false);
    }

    public void UpdateCurrentObjectiveScore(int newScore = int.MinValue) {
        if (!int.TryParse(curObjectiveText.text, out int score))
            score = -1;
        curScore = score + (newScore == int.MinValue ? 1 : newScore);
        curObjectiveText.text = curScore.ToString();
        if (curScore >= curObjectiveMax)
            GameManager.Instance.finishedObjectives = true;
    }
}
