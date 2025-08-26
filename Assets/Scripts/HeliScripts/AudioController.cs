using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {
    static public AudioController Instance { get; private set; }
    AudioSource backgroundAudioSource = null, heliAudioSource = null;

    public enum BackgroundTypes { Background, Level };
    public enum LevelTypes { Crash, Heli };
    public AudioClip backgroundAudio, levelAudio, heliBlades, heliCrash;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            backgroundAudioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            SetBackgroundAudio(BackgroundTypes.Background);
        }
    }

    public void SetHeliAudioSource(AudioSource source) { heliAudioSource = source; }

    public void SetBackgroundAudio(BackgroundTypes type) {
        backgroundAudioSource.Stop();
        switch (type) {
            case BackgroundTypes.Background:
                if (backgroundAudioSource.clip != backgroundAudio)            
                    backgroundAudioSource.clip = backgroundAudio;
                break;
            case BackgroundTypes.Level:
                if (backgroundAudioSource.clip != levelAudio)
                    backgroundAudioSource.clip = levelAudio;
                break;
        }
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    public void StopBackgroundAudio() { backgroundAudioSource.Stop(); }

    public void StopHeliAudio() { heliAudioSource.Stop(); }

    public void StartBackgroundAudio() { backgroundAudioSource.Play(); }

    public void StartHeliAudio() { heliAudioSource.Play(); }

    public void FadeAudio(float fadeDuration, BackgroundTypes type) { StartCoroutine(Fade(fadeDuration, type)); }

    public void FadeAudioIn(float fadeDuration, LevelTypes type) { StartCoroutine(Fade(fadeDuration, type)); }

    IEnumerator Fade(float fadeDuration, BackgroundTypes type) {
        print(type);
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
        float startVolume = heliAudioSource.volume;
        while (heliAudioSource.volume < startVolume) {
            heliAudioSource.volume += startVolume / Time.deltaTime * fadeDuration;
            yield return null;
        }
        heliAudioSource.volume = startVolume;
    }
    
    public void PlayAudio(LevelTypes type) {
        switch (type) {
            case LevelTypes.Crash:
                heliAudioSource.PlayOneShot(heliCrash);
                break;
            case LevelTypes.Heli:
                heliAudioSource.volume = .75f;
                heliAudioSource.clip = heliBlades;
                heliAudioSource.loop = true;
                heliAudioSource.Play();
                break;
        }
    }
}
