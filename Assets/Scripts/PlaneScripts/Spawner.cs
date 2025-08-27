using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject spawnPOS, plane;
    [SerializeField] bool enemySpawner;
    [SerializeField] float timer = 5f;
    float time = 0f;

    
    void Update() 
    {
        if (GameManager.Instance.currentManager is PlaneGameManager planeManager)
        {
            if (enemySpawner && planeManager.enemies >= planeManager.enemiesMax) return;
            else if (!enemySpawner && planeManager.friendlies >= planeManager.friendliesMax) return;
            time += Time.deltaTime;
            if(time > timer)
            {
                if (enemySpawner) planeManager.enemies++;
                else planeManager.friendlies++;
                Instantiate(plane, spawnPOS.transform.position, spawnPOS.transform.rotation, spawnPOS.transform.parent);
                time = 0f;
            }
        }
    }
}
