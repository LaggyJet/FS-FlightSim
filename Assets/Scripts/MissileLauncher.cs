using System.Collections;
using TMPro;
using UnityEngine;

public class MissileLauncher : MonoBehaviour {
    public static MissileLauncher Instance { get; private set; }

    [SerializeField] GameObject missile, warningTextContainter;
    [SerializeField] TMP_Text warningText, countdownText;
    [SerializeField] float warningDuration = 5f;

    GameObject target, activeMissile;
    float countdown;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void SpawnMissile() {
        if (activeMissile == null) {
            activeMissile = Instantiate(missile, transform.position, transform.rotation);
            activeMissile.SetActive(false);
        }
    }

    public void LaunchMissile(GameObject trackedObject) {
        SpawnMissile();          
        target = trackedObject;  
        StartCoroutine(WarningSequence());
    }

    IEnumerator WarningSequence() {
        warningTextContainter.SetActive(true);
        countdown = warningDuration;
        warningText.text = "Entering restricted airspace, lower altitude or be shot down.";
        warningText.color = Color.red;
        countdownText.gameObject.SetActive(true);
        countdownText.enabled = true;
        while (countdown > 0) {
            countdownText.text = countdown.ToString("F0");
            yield return new WaitForSeconds(1f);
            warningText.enabled = !warningText.enabled;
            countdown--;
        }
        countdownText.gameObject.SetActive(false);
        warningText.enabled = true;
        warningText.text = "Incoming Missile, lower altitiude.";
        countdownText.enabled = false;
        ActivateMissile(); 
    }

    void ActivateMissile() {
        if (activeMissile != null) {
            activeMissile.SetActive(true);
            if (activeMissile.TryGetComponent<MissileTracker>(out var missileTracker))
                missileTracker.SetTarget(target);
        }
    }

    public void CancelMissile(GameObject trackedObject) {
        warningTextContainter.SetActive(false);
        if (activeMissile != null && target == trackedObject) {
            if (activeMissile.TryGetComponent<MissileTracker>(out var missileTracker))
                missileTracker.SetTarget(null);
            Destroy(activeMissile);
            activeMissile = null; 
            target = null; 
            countdown = 0;
        }
    }
}