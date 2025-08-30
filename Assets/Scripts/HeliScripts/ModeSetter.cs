using Unity.VisualScripting;
using UnityEngine;

public class ModeSetter : MonoBehaviour {
    [SerializeField] GameObject[] modes;

    void Start() {
        if (GameManager.Instance.currentModeManager is HeliGameManager heliManager) {
            heliManager.modes = modes;
            UI.Instance.SetHeliObjectives();
            GameManager.Instance.StartGameMode();
        }
    }
}
