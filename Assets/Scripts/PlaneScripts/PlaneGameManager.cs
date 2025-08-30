using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Plane Manager", menuName = "GameManagers/Plane Manager")]
public class PlaneGameManager : BaseModeManager {
    //Debug
    [SerializeField, HideInDebugUI] bool debug;

    [Tooltip("DogFight Variables")]
    public int enemies = 0;
    public int enemiesKilled = 0;
    public int enemiesMax = 100;
    public int friendlies = 0;
    public int friendliesKilled = 0;
    public int friendliesMax = 100;
    public bool runTimer = false;
    public float timeNow = 0f;
    public float timeMax = 300f;

    public override void ModeUpdate() {
        if (debug) {
            for (int i = 0; i < 20; i++)
                if (Input.GetKeyDown("joystick button " + i))
                    UnityEngine.Debug.Log("Button " + i + " was pressed!");
        }
        if (runTimer) { timeNow += Time.deltaTime; UI.Instance.SetTimer(timeMax - timeNow); }
        if (Input.GetButtonDown("Cancel") && GameManager.Instance.selectedGameMode != GameMode.None) GameManager.Instance.OnPauseResume();
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