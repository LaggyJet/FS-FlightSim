using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int force = 2000;
    [SerializeField] float lifeSpan = 5.0f;
    [SerializeField] float damage = 25;
    Collider coll;
    Rigidbody rb;
    public GameObject parent = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        rb.AddForce(this.transform.forward * force,  ForceMode.Impulse);
        Destroy(this.gameObject, lifeSpan);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<PlaneAi>()?.TakeDamage(damage, parent);
    }
}
