using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlaneAi : MonoBehaviour
{
    [Header("Boid Variables")]
    [SerializeField] bool enemyAi; //determines which 'team' the boid is on
    [SerializeField] Transform LOS; //our point of view for the plane
    [SerializeField] LayerMask mask; //what it needs to be able to detect
    [SerializeField] float sightDistance = 100f; //how far the plane can see in front of it
    List<GameObject> found = new List<GameObject>();
    List<GameObject> boids = new List<GameObject>();
    List<Vector3> avoid = new List<Vector3>();
    [SerializeField, HideInDebugUI] int rayCount = 100;
    [SerializeField, HideInDebugUI] float density = 2;
    [SerializeField, HideInDebugUI] float separation = .68f;

    [Header("Plane Stats")]
    [SerializeField] float hp;
    [SerializeField] float speed = 80f;
    [SerializeField] float responsiveness = 15f;
    [SerializeField] float gravity = 100f;
    [SerializeField] int accuracy = 10;
    GunSystem weapons;
    Rigidbody rb;
    [SerializeField, HideInDebugUI] bool crashing = false;

    //Debug Variables
    [SerializeField, HideInDebugUI] bool drawLines = false;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<GunSystem>();
        responsiveness = Mathf.Pow(responsiveness, 3);
    }

    private void FixedUpdate()
    {
        if (!crashing) rb.AddForce((rb.transform.forward * speed), ForceMode.Acceleration);
        else Crash();
    }
    private void Update()
    {
        if(!crashing)
        {
            found.Clear();
            boids.Clear();
            avoid.Clear();
            FindObstacle();
            FollowBoid();
            AvoidObstacle();
        }
    }

    private void FindObstacle()
    {
        //shoot rays out in a cone for finding boids
        for(int z = 0; z < rayCount; z++)
        {
            float dist = z / (rayCount - 1f);
            float angle = 2 * Mathf.PI * separation * z;
            float x = dist * Mathf.Cos(angle) / density;
            float y = dist * Mathf.Sin(angle) / density;

            Vector3 plot = new Vector3(LOS.forward.x + x , LOS.forward.y + y + Mathf.Abs(LOS.rotation.x), LOS.forward.z + LOS.rotation.z) * sightDistance;

            //draws our rays being shot and if one hits an object we find its root gameobject and search through our list of gameobjects, if its found we return
            //but if its not we add it to the list then determine if its an obstacle or a boid and add it to the appropriate list for later use
            if (drawLines)
            {
                UnityEngine.Color color;
                if (z < accuracy) color = UnityEngine.Color.blue; else color = UnityEngine.Color.yellow;
                Debug.DrawRay(LOS.position, plot, color);
            }
            if (Physics.Raycast(LOS.position, plot, out RaycastHit hit, sightDistance))
            {
                //draws a red line for each detected object
                if (drawLines) Debug.DrawLine(LOS.position, hit.point, UnityEngine.Color.red);

                GameObject temp = hit.transform.root.gameObject;
                if (found.Find(o => o == temp) == null) { found.Add(temp); }
                else return;
                if (hit.collider.gameObject.GetComponent<PlaneAi>())
                {
                    boids.Add(hit.collider.gameObject.transform.root.gameObject);
                    if (IsEnemy(hit.collider.gameObject.transform.root.gameObject.tag) && z < accuracy) weapons.Fire();
                }
                else avoid.Add(hit.point);

                
            }
        }
        
    }


    private void FollowBoid()
    {
        //if the list of boids is empty we return out
        if (boids.Count == 0) return;

        //temp variables being declared
        Vector3 closest = Vector3.zero;
        float distance = float.MaxValue;
        
        //looping through the boids list and checking if the boid found is an enemy or friendly by passing the objects tag into the function
        foreach(GameObject plane in boids)
        {
            if (IsEnemy(plane.gameObject.tag))
            {
                float temp = Vector3.Distance(rb.transform.position, plane.transform.position);
                //if all  requirements are met we make this the new closest and track the distance as this will be used to adjust the plane following and how fast it follows
                //we add our old position to avoid list as we will now want to avoid it
                if (temp > distance)
                {
                    distance = temp;
                    avoid.Add(closest);
                    closest = plane.transform.position;
                }
                //else we add the point to our avoid list for use later
                else
                {
                    avoid.Add(plane.transform.position);
                }
            }
            else
            {
                avoid.Add(plane.transform.position);
            }
        }   
        
        //makes our point to follow be reactive to our objects velocity and distance from the point to make movements seem more natural
        Vector3 followDirection = rb.linearVelocity + closest.normalized * (1/distance);
        transform.LookAt(followDirection);
    }

    private void AvoidObstacle()
    {
        //if avoid list is empty return out
        if(avoid.Count == 0) return;
        
        //declares a vector that will be the inverse of the average of obstacles to avoid
        Vector3 safeDirection = Vector3.zero;
        //loops through points to avoid and adds the inverse divided by their distance to the safeDirection vector
        foreach (Vector3 point in avoid)
        {
            float distance = Vector3.Distance(rb.transform.position, point);
            safeDirection += (-point) / Mathf.Sqrt(distance);
        }
        //averages the vector to account for many points being added then multiplies it by our responsiveness to adjust how fast the plane will turn to avoid the obstacles
        safeDirection /= avoid.Count;
        safeDirection *= responsiveness;

        //if the safe direction is further on the z axis we want to only apply pitch otherwise we only apply yaw, this keeps the movement more natural and less erratic
        float scalex = safeDirection.x / safeDirection.y;
        float scaley = safeDirection.y / safeDirection.x;
        rb.AddTorque(new Vector3((rb.transform.position.x + safeDirection.x) * scaley, (rb.transform.position.y + safeDirection.y) * scalex, 0));
        //else if (safeDirection.x > safeDirection.y) rb.AddTorque(new Vector3(rb.transform.position.x + safeDirection.x, 0, 0));
        //else rb.AddTorque(new Vector3(0, rb.transform.position.y + safeDirection.y, 0));
    }


    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) crashing = true;
    }

    public void Crash()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        if (rb.angularVelocity.x <= 0) { rb.AddTorque(new Vector3(0, 100 * -responsiveness, 0)); }
        else { rb.AddTorque(new Vector3( 0, 100 * responsiveness, 0)); }
    }

    private bool IsEnemy(string tag)
    {
        if (enemyAi && tag == "Corsair")
            return true;
        else if (!enemyAi && tag == "Zero")
            return true;
        else return false;
    }

    //Rules for BOIDS
    //1.) Avoid Collision With Other Boids
    //2.) Try and Align to Center of Opposing Boids
    //3.) Start Firing if Opposing Boid is In Range
    //4.) Avoid Non-Boid Obstacles
}
