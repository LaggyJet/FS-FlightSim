using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    Resolution[] resolutions;
    [SerializeField] TMP_Dropdown resDropDown;

    void Start() { 
        DontDestroyOnLoad(gameObject);
        resolutions = Screen.resolutions;
        resDropDown.ClearOptions();
        List<string> options = new();
        int curResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++) {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                curResIndex = i;
        }
        resDropDown.AddOptions(options);
        resDropDown.value = curResIndex;
        resDropDown.RefreshShownValue();
    }

    public void SetRes(int resIndex) { Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen); }
}
