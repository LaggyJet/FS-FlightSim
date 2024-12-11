using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject spawn;
    [SerializeField] bool shoot = false;
    float nextfire = 0;
    float fire = 0.02f;

    private void Update()
    {
        if (shoot && Time.time > nextfire)
        {
            Instantiate(projectile, spawn.transform.position, spawn.transform.rotation);
            nextfire = Time.time + fire;
        }
    }
}
