using System.Collections;
using TMPro;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField] bool limit;

    void OnTriggerEnter(Collider other)
    {
        if (limit) { UI.Instance.ShowWarning(false); other.gameObject.GetComponent<PlaneController>()?.Explode(); }
        PlaneController player = other.transform.root.gameObject.GetComponent<PlaneController>();
        if (player)
        {
            UI.Instance.ShowWarning(true);
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        PlaneController player = other.gameObject.GetComponent<PlaneController>();
        if (player)
        {
            UI.Instance.ShowWarning(false);
        }
    }
}