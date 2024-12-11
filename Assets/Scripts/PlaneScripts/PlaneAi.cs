using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaneAi : MonoBehaviour
{
    [SerializeField] Transform LOS;
    [SerializeField] LayerMask mask;
    List<GameObject> boids;
    List<Position> avoid;

    private IEnumerator moveObstacles()
    {
        yield return null;
    }

    private List<GameObject> findObstacle()
    {
        boids.Clear();

        //shoot rays out in a cone for finding boids
        for(int x = 0; x < 100; x++)
        {
            if (Physics.Raycast(LOS.position, LOS.forward, out RaycastHit hit, float.MaxValue, mask))
                if (hit.collider.GetComponent<PlaneAi> != null);
                boids.Add(hit.collider.gameObject.transform.root.gameObject);
        }
        
    }





    //Rules for BOIDS
    //1.) Avoid Collision With Other Boids
    //2.) Try and Align to Center of Opposing Boids
    //3.) Start Firing if Opposing Boid is In Range
    //4.) Avoid Non-Boid Obstacles
}
