using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlaneGameManager : GameManager {
    //Debug
    [SerializeField, HideInDebugUI] bool debug;

    [Tooltip("DogFight Variables")]
    public int enemies = 0;
    public int enemiesKilled = 0;
    [SerializeField] public int enemiesMax = 100;
    public int friendlies = 0;
    public int friendliesKilled = 0;
    [SerializeField] public int friendliesMax = 100;
    public bool runTimer = false;
    public float timeNow = 0f;

    void Update() {
        if (debug) {
            for (int i = 0; i < 20; i++)
                if (Input.GetKeyDown("joystick button " + i))
                    UnityEngine.Debug.Log("Button " + i + " was pressed!");
        }
        if (runTimer) { timeNow += Time.deltaTime; UI.Instance.SetTimer(timeMax - timeNow); }
        if (Input.GetButtonDown("Cancel") && selectedGameMode != GameMode.None) OnPauseResume();
    }

    public override IEnumerator WinGame() {
        //EventSystem.current.SetSelectedGameObject(UIUpdater.Instance.winButton);
        yield return new WaitForSeconds(3f);
        //UIUpdater.Instance.background.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        //UIUpdater.Instance.winMenu.SetActive(true);

        //update the scores
    }

    public override IEnumerator LoseGame() {
        UI.Instance.PlaneGameOver();
        yield return null;
    }

    public override void ResetVals() {
        timeNow = 0;
        runTimer = false;
        enemies = 0;
        friendlies = 0;
        enemiesKilled = 0;
        friendliesKilled = 0;
    }
}