using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int force = 2000;
    [SerializeField] float lifeSpan = 5.0f;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(this.transform.forward * force,  ForceMode.Impulse);
        Destroy(this.gameObject, lifeSpan);
    }
}
