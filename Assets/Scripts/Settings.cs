using System.Linq;
using TMPro;
using UnityEngine;

public class Settings : MonoBehaviour {
    public static Settings Instance {  get; private set; }
    public enum GameMode { None, TimeAttack, FreeFlight, ObstacleCourse };
    public GameMode selectedGameMode = GameMode.None;

    [SerializeField] TMP_Dropdown resDropDown;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        resDropDown.ClearOptions();
        resDropDown.AddOptions(Screen.resolutions.Select(res => $"{res.width} x {res.height}").ToList());
        resDropDown.value = Screen.resolutions.Select((res, index) => new {res, index}).FirstOrDefault(pair => pair.res.width == Screen.currentResolution.width && pair.res.height == Screen.currentResolution.height)?.index ?? 0;
        resDropDown.RefreshShownValue();
    }

    public void SetRes(int resIndex) { Screen.SetResolution(Screen.resolutions[resIndex].width, Screen.resolutions[resIndex].height, Screen.fullScreen); }
}
