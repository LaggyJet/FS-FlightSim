using System;
using System.Collections;
using UnityEngine;

public struct GameMode {
    public enum Category { None, Plane, Heli }
    public enum PlaneMode { DogFight, LandingTrial }
    public enum HeliMode { FreeFlight, ObstacleCourse, TimeAttack }

    public Category category;
    public int mode;

    public static GameMode None => new() { category = Category.None };
    public static GameMode DogFight => new() { category = Category.Plane, mode = (int)PlaneMode.DogFight };
    public static GameMode LandingTrial => new() { category = Category.Plane, mode = (int)PlaneMode.LandingTrial };
    public static GameMode FreeFlight => new() { category = Category.Heli, mode = (int)HeliMode.FreeFlight };
    public static GameMode ObstacleCourse => new() { category = Category.Heli, mode = (int)HeliMode.ObstacleCourse };
    public static GameMode TimeAttack = new() { category = Category.Heli, mode = (int)HeliMode.TimeAttack };

    public readonly PlaneMode AsPlane => (PlaneMode)mode;
    public readonly HeliMode AsHeli => (HeliMode)mode;

    public override readonly bool Equals(object obj) => obj is GameMode other && category == other.category && mode == other.mode;

    public override readonly int GetHashCode() => (category, mode).GetHashCode();

    public static bool operator ==(GameMode a, GameMode b) => a.Equals(b);
    public static bool operator !=(GameMode a, GameMode b) => !a.Equals(b);

    public override readonly string ToString() =>
        category switch {
            Category.Plane => AsPlane.ToString(),
            Category.Heli => AsHeli.ToString(),
            _ => "None"
        };

    public readonly BaseModeManager GetManagerType() {
        return category switch { 
            Category.Plane => ScriptableObject.CreateInstance<PlaneGameManager>(),
            Category.Heli => ScriptableObject.CreateInstance<HeliGameManager>(),
            _ => null
        };
    }
}

public class GameManager : MonoBehaviour {    
    //Singleton
    public static GameManager Instance { get; private set; }


    [Tooltip("Running Variables")]
    public bool isPaused = false;


    [Header("Game Important Variables")]
    public GameMode selectedGameMode = GameMode.None;
    public BaseModeManager currentModeManager;

    public void SetGameManager(GameMode gameMode) {
        selectedGameMode = gameMode;
        var managerType = gameMode.GetManagerType();
        if (managerType != null)
            currentModeManager = managerType;
        if (currentModeManager is HeliGameManager)
            UI.Instance.SetHeliUI();
        else
            UI.Instance.SetPlaneUI();
    }

    void Awake() {
        //Singleton code
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        EnableGameMode(GameMode.None);
        
        //TODO: Fix once I figure out how
        //EventSystem.current.SetSelectedGameObject(pauseButton);
    }

    void Update() { 
        if (currentModeManager)
            currentModeManager.ModeUpdate(); 
    }

    void Start() {
        //EnableGameMode(selectedGameMode);
        //ResumeGame();
    }

    public void EnableGameMode(GameMode gameMode) {
        if (gameMode.category == GameMode.Category.Heli && gameMode.AsHeli == GameMode.HeliMode.FreeFlight) {
            //TODO: Also fix
            //objectivesMenu.SetActive(false);
            //TEMP FIX I think
            //UI.Instance.gameModeUI.gameObject.SetActive(false);
        }
    }

    public void StartGameMode() {
        if (currentModeManager is HeliGameManager heliManager) {
            for (int i = 0; i < heliManager.modes.Length; i++) {
                heliManager.modes[i].SetActive(selectedGameMode.ToString() == heliManager.modes[i].name);
                heliManager.uiSettings[i].SetActive(selectedGameMode.ToString() == heliManager.uiSettings[i].name);
            }
        }
    }

    public void OnPauseResume() { print("paused"); (Instance.isPaused ? (Action)Instance.ResumeGame : Instance.PauseGame)(); }

    public void PauseGame()
    {
        if (selectedGameMode == GameMode.None) return;
        UI.Instance.Pause();
        Stop();


        isPaused = true;
        UI.Instance.pauseBackground.SetActive(true);
        UI.Instance.generalMenus[(int)GeneralMenuIndex.PAUSE].SetActive(isPaused);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Stop() {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame() {
        UI.Instance.UnPause();

        UI.Instance.titleMenus[(int)TitleMenuIndex.SETTINGS].SetActive(false);
        isPaused = false;
        UI.Instance.pauseBackground.SetActive(false);
        UI.Instance.generalMenus[(int)GeneralMenuIndex.PAUSE].SetActive(isPaused);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Continue()
    {
        UI.Instance.titleMenus[1].SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void CallWinGame() { StartCoroutine(currentModeManager?.WinGame()); }

    public virtual IEnumerator WinGame() { yield break; }

    public void CallLoseGame() { StartCoroutine(currentModeManager?.LoseGame()); }

    public virtual IEnumerator LoseGame() { yield break; }
}
