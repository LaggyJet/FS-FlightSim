using UnityEngine;

public class MissileTracker : MonoBehaviour {
    GameObject target;

    void Update() {
        if (target != null) {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            transform.SetPositionAndRotation(transform.position + (20f * Time.deltaTime * directionToTarget), Quaternion.LookRotation(directionToTarget));
        }
    }

    public void SetTarget(GameObject newTarget) { target = newTarget; }

    void OnCollisionEnter(Collision collision) { if (collision.gameObject == target) Destroy(gameObject); }
}
