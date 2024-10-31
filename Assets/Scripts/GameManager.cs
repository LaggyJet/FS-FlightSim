using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance {  get; private set; }
    [SerializeField] GameObject[] modes;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        EnableGameMode(Settings.GameMode.None); 
    }

    void Start() {
        switch (Settings.Instance.selectedGameMode) {
            case Settings.GameMode.TimeAttack:
                EnableGameMode(Settings.GameMode.TimeAttack);
                break;
            case Settings.GameMode.FreeFlight:
                EnableGameMode(Settings.GameMode.FreeFlight);
                break;
            case Settings.GameMode.ObstacleCourse:
                EnableGameMode(Settings.GameMode.ObstacleCourse);
                break;
            default:
                break;
        }
    }

    void EnableGameMode(Settings.GameMode gameMode) {
        foreach (var mode in modes) {
            if (gameMode.ToString() == mode.ToString().Split(' ')[0])
                mode.SetActive(true);
            else
                mode.SetActive(false);
        }
    }
}
