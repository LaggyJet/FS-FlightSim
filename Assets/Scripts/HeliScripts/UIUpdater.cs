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
        if (GameManager.Instance.currentManager is HeliGameManager heliManager) {
            List<Transform> tfs = new();
            switch (GameManager.Instance.selectedGameMode) {
                case GameManager.GameMode.TimeAttack:
                    curObjectiveObject = GameManager.Instance.uiSettings[0];
                    //TODO: make sure to update find when adding time attack
                    curObjectiveText = curObjectiveObject.transform.Find("").GetComponent<TMP_Text>();
                    curObjectiveMax = GameManager.Instance.modes[0].transform.childCount;
                    for (int i = 0; i < curObjectiveMax; i++)
                        tfs.Add(GameManager.Instance.modes[0].transform.GetChild(i));
                    break;
                case GameManager.GameMode.ObstacleCourse:
                    curObjectiveObject = GameManager.Instance.uiSettings[1];
                    curObjectiveText = curObjectiveObject.transform.Find("LandingZones/Completed").GetComponent<TMP_Text>();
                    curObjectiveMax = GameManager.Instance.modes[1].transform.childCount;
                    for (int i = 0; i < curObjectiveMax; i++)
                        tfs.Add(GameManager.Instance.modes[1].transform.GetChild(i));
                    break;
            }
            heliManager.objectivesCompleted = new System.Tuple<GameObject, bool>[curObjectiveMax];
            heliManager.objectiveAccuries = new float[curObjectiveMax];
            for (int i = 0; i < tfs?.Count; i++)
                    heliManager.objectivesCompleted[i] = Tuple.Create(tfs[i].gameObject, false);
        }
    }

    public void UpdateCurrentObjectiveScore(int newScore = int.MinValue) {
        if (!int.TryParse(curObjectiveText.text, out int score))
            score = -1;
        curScore = score + (newScore == int.MinValue ? 1 : newScore);
        curObjectiveText.text = curScore.ToString();
        if (curScore >= curObjectiveMax && GameManager.Instance.currentManager is HeliGameManager heliManager)
            heliManager.finishedObjectives = true;
    }
}
