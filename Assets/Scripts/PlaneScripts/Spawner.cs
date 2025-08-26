using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject spawnPOS, plane;
    [SerializeField] bool enemySpawner;
    [SerializeField] float timer = 5f;
    float time = 0f;

    
    void Update()
    {
        if (enemySpawner && GameManager.Instance.enemies >= GameManager.Instance.enemiesMax) return;
        else if (!enemySpawner && GameManager.Instance.friendlies >= GameManager.Instance.friendliesMax) return;
        time += Time.deltaTime;
        if(time > timer)
        {
            if (enemySpawner) GameManager.Instance.enemies++;
            else GameManager.Instance.friendlies++;
            Instantiate(plane, spawnPOS.transform.position, spawnPOS.transform.rotation, spawnPOS.transform.parent);
            time = 0f;
        }
    }
}
