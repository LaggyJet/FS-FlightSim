using UnityEngine;

public class HelicopterPartCollisionHandler : MonoBehaviour {
    public HelicopterController helicopterController;
    public enum PartType { Rotor, Body, SkiL, SkiR }
    public PartType partType;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Missile"))
            helicopterController.Explode();
        else if (!other.transform.IsChildOf(transform) && !other.CompareTag("BoundingWall"))
            helicopterController.HandlePartCollision(partType, other);
    }
}