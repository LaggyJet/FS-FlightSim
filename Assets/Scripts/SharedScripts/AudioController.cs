using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {
    static public AudioController Instance { get; private set; }
    AudioSource backgroundAudioSource = null, heliLevelAudioSource = null;

    public enum BackgroundTypes { Menu, Level };
    public enum LevelTypes { Crash, Heli };
    [Header("General Music")]
    public AudioClip mainScreenAudio;
    [Space]
    [Header("Helicopter Audio")]
    public AudioClip heliLevelAudio;
    public AudioClip heliBlades;
    public AudioClip heliCrash;
    [Space]
    [Header("Plane Audio")]
    public AudioClip planeGunShots;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            backgroundAudioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            SetBackgroundAudio(BackgroundTypes.Menu);
        }
    }

    public void SetHeliAudioSource(AudioSource source) { heliLevelAudioSource = source; }

    public void SetBackgroundAudio(BackgroundTypes type) {
        backgroundAudioSource.Stop();
        switch (type) {
            case BackgroundTypes.Menu:
                if (backgroundAudioSource.clip != mainScreenAudio)
                    backgroundAudioSource.clip = mainScreenAudio;
                break;
            case BackgroundTypes.Level:
                if (backgroundAudioSource.clip != heliLevelAudio)
                    backgroundAudioSource.clip = heliLevelAudio;
                break;
        }
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public void StopBackgroundAudio() { backgroundAudioSource.Stop(); }

    public void StopHeliAudio() { heliLevelAudioSource.Stop(); }

    public void StartBackgroundAudio() { backgroundAudioSource.Play(); }

    public void StartHeliAudio() { heliLevelAudioSource.Play(); }

    public void FadeAudio(float fadeDuration, BackgroundTypes type) { StartCoroutine(Fade(fadeDuration, type)); }

    public void FadeAudioIn(float fadeDuration, LevelTypes type) { StartCoroutine(Fade(fadeDuration, type)); }

    IEnumerator Fade(float fadeDuration, BackgroundTypes type) {
        float startVolume = backgroundAudioSource.volume;
        while (backgroundAudioSource.volume > 0.05f) {
            backgroundAudioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        backgroundAudioSource.volume = 0;
        SetBackgroundAudio(type);
        while (backgroundAudioSource.volume < startVolume) {
            backgroundAudioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
        backgroundAudioSource.volume = startVolume;
    }

    IEnumerator Fade(float fadeDuration, LevelTypes type) {
        float startVolume = heliLevelAudioSource.volume;
        while (heliLevelAudioSource.volume < startVolume) {
            heliLevelAudioSource.volume += startVolume / Time.deltaTime * fadeDuration;
            yield return null;
        }
        heliLevelAudioSource.volume = startVolume;
    }
    
    public void PlayAudio(LevelTypes type) {
        switch (type) {
            case LevelTypes.Crash:
                heliLevelAudioSource.PlayOneShot(heliCrash);
                break;
            case LevelTypes.Heli:
                heliLevelAudioSource.volume = .2f;
                heliLevelAudioSource.clip = heliBlades;
                heliLevelAudioSource.loop = true;
                heliLevelAudioSource.Play();
                break;
        }
    }

    public void DecreaseAudio(int amount) { backgroundAudioSource.volume -= amount/100.0f; }

    public void IncreaseAudio(int amount) { backgroundAudioSource.volume += amount/100.0f; }

    public void SetBackgroundAudioVolume(float newVolume) { backgroundAudioSource.volume = newVolume; }
}
