using UnityEngine;

public class HelicopterPartCollisionHandler : MonoBehaviour {
    public HelicopterController helicopterController;
    public enum PartType { Rotor, Body, SkiL, SkiR }
    public PartType partType;

    private void OnTriggerEnter(Collider other) {
        if (!other.transform.IsChildOf(transform))
            helicopterController.HandlePartCollision(partType, other);
    }
}