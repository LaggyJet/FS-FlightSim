using System.Collections;
using UnityEngine;

[System.Serializable]
public class BaseModeManager : ScriptableObject {
    public virtual void ModeUpdate() { return; }

    public virtual IEnumerator WinGame() { yield break; }

    public virtual IEnumerator LoseGame() { yield break; }

    public virtual void ResetVals() { }
}
