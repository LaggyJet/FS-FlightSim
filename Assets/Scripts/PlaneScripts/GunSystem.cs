using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class GunSystem : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.02f;
    [SerializeField] private bool hitscan;
    [SerializeField] private ParticleSystem bulletEffect;
    [SerializeField] private ParticleSystem bulletImpact;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private LayerMask mask;
    [SerializeField] private Transform[] spawns;
    [SerializeField] private GameObject bullet;
    private float nextfire = 0;
    private int x = 0;

    [SerializeField, HideInDebugUI] private bool shoot;

    private void Update()
    {
        if(shoot)
        {
            Fire();
        }
    }

    public void Fire()
    {
        if(Time.time > nextfire && hitscan)
        {
            foreach (var spawn in spawns)
            {
                if(Physics.Raycast(spawn.position, spawn.forward, out RaycastHit hit, float.MaxValue, mask))
                {
                    TrailRenderer trail = Instantiate(bulletTrail, spawn.position, Quaternion.identity);

                    StartCoroutine(SpawnTrail(trail, hit));
                }
            }
            nextfire = Time.time + fireRate;
        }
        else if (Time.time > nextfire)
        {
            Instantiate(bullet, spawns[x].transform.position, spawns[x].transform.rotation);
            if (x == 5) { x = 0; }
            else x++;
            nextfire = Time.time + fireRate;
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);

            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;
        Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(trail.gameObject, trail.time);
    }

}
