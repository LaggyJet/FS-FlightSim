using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    public static Settings Instance { get; private set; }

    [Header("Motion Platform Settings")]
    public string port = "COM3";
    public int baud = 115200;
    [Space]

    [Header("Resolution")]
    [SerializeField] TMP_Dropdown resDropDown;
    [Space]

    [Header("Background Music Volume")]
    [SerializeField] Slider backgroundMusicSlider;
    [SerializeField] TMP_Text curBackgroundMusicVolText;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        //Setting up Motion Platform (If Connected)
        PlatformController.singleton.Init(port, baud);

        //Setting up Resolution
        resDropDown.ClearOptions();
        resDropDown.AddOptions(Screen.resolutions.Select(res => $"{res.width} x {res.height}").ToList());
        resDropDown.value = Screen.resolutions.Select((res, index) => new { res, index }).FirstOrDefault(pair => pair.res.width == Screen.currentResolution.width && pair.res.height == Screen.currentResolution.height)?.index ?? 0;
        resDropDown.RefreshShownValue();

        //Setting up Music Volume

    }

    //Resolution Info
    public void SetRes(int resIndex) { Screen.SetResolution(Screen.resolutions[resIndex].width, Screen.resolutions[resIndex].height, Screen.fullScreen); }

    //Music Volume
    public void SetBackgroundVolume() {
        float newVolume = backgroundMusicSlider.value;
        AudioController.Instance.SetBackgroundAudioVolume(newVolume/100.0f);
        curBackgroundMusicVolText.text = newVolume.ToString();
    }
}
