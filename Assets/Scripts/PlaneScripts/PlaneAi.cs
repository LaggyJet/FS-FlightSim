using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PlaneAi : MonoBehaviour
{
    [Header("Boid Variables")]
    [SerializeField] bool enemyAi; //determines which 'team' the boid is on
    [SerializeField] Transform LOS; //our point of view for the plane
    [SerializeField] LayerMask mask; //what it needs to be able to detect
    [SerializeField] float sightDistance = 100f; //how far the plane can see in front of it
    [SerializeField] float randomnessVal = 0;
    [SerializeField] bool random = false;
    List<GameObject> found = new List<GameObject>();
    List<GameObject> boids = new List<GameObject>();
    List<Vector3> avoid = new List<Vector3>();
    [SerializeField, HideInDebugUI] int rayCount = 100;
    [SerializeField, HideInDebugUI] float density = 2;
    [SerializeField, HideInDebugUI] float separation = .68f;
    [SerializeField, HideInDebugUI] GameObject explosion;

    [Header("Plane Stats")]
    [SerializeField] float hp;
    [SerializeField] float speed = 80f;
    [SerializeField] int responsiveness = 100;
    [SerializeField] float gravity = 100f;
    [SerializeField] int accuracy = 10;
    GunSystem weapons;
    Rigidbody rb;
    [SerializeField, HideInDebugUI] bool crashing = false;

    //Debug Variables
    [SerializeField, HideInDebugUI] bool drawLines = false;
    [SerializeField, HideInDebugUI] GameObject debugCube;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<GunSystem>();
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
            if (!FollowBoid() && !AvoidObstacle()) RandFlight();
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

            Vector3 m = transform.TransformDirection(new Vector3(x, y, 0));

            Vector3 plot = new Vector3(LOS.forward.x, LOS.forward.y, LOS.forward.z);
            plot += m;
            plot *= sightDistance;

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

                if (hit.collider.gameObject.GetComponent<PlaneAi>() || hit.collider.gameObject.transform.root.GetComponent<PlaneController>())
                {
                    boids.Add(hit.collider.gameObject.transform.root.gameObject);
                    //if (IsEnemy(hit.collider.gameObject.transform.root.gameObject.tag) && z < accuracy) weapons.Fire();
                }
                else avoid.Add(hit.point);
            }
        }
        
    }


    private bool FollowBoid()
    {
        //if the list of boids is empty we return out
        if (boids.Count == 0) return false;
        
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
                if (temp < distance)
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


        if (drawLines) Debug.DrawRay(LOS.position, closest, UnityEngine.Color.white);
        //gives us the translation from our avoid position to where we are essentially telling us where we want to go
        Vector3 translation = LOS.position - closest;
        //this torque vector reverses our x and y so that way we get the desired rotation and we do the opposite on the z axis to avoid any induced roll
        Vector3 torque = new Vector3(translation.y, translation.x, -(translation.x + translation.y)) * responsiveness;

        rb.AddTorque(torque);

        return true;
    }

    private bool AvoidObstacle()
    {
        ////if avoid list is empty return out
        if(avoid.Count == 0) return false;
        
        //declares a vector that will be the inverse of the average of obstacles to avoid
        Vector3 avoidDirection = Vector3.zero;
        //loops through points to avoid and adds the position to avoid
        foreach (Vector3 point in avoid)
        {
            avoidDirection += (point);
        }
        //gives us our average position of objects we want to avoid
        avoidDirection /= avoid.Count;
        if(drawLines) Debug.DrawRay(LOS.position, avoidDirection, UnityEngine.Color.black);
        float counterRoll = -rb.angularVelocity.z;
        //this torque vector reverses our x and y so that way we get the desired rotation and we do the opposite on the z axis to avoid any induced roll
        Vector3 torque = new Vector3(avoidDirection.y, avoidDirection.x, counterRoll) * responsiveness;
        
        rb.AddTorque(torque);

        return true;
    }

    private void RandFlight()
    {
        if (random)
        {
            float randx;
            float randy;
            if (rb.linearVelocity.x > 0) randx = UnityEngine.Random.Range(-randomnessVal, randomnessVal * 2);
            else randx = UnityEngine.Random.Range(-randomnessVal * 2, randomnessVal);
            if (rb.linearVelocity.y > 0) randy = UnityEngine.Random.Range(-randomnessVal, randomnessVal * 2);
            else randy = UnityEngine.Random.Range(-randomnessVal * 2, randomnessVal);

            rb.AddTorque(new Vector3((rb.transform.position.x + randx), (rb.transform.position.y + randy), 0));
        }
    }


    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (!crashing && hp <= 0) crashing = true;
    }

    public void TakeDamage(float damage, GameObject owner)
    {
        hp -= damage;
        if (!crashing && hp <= 0)
        {
            crashing = true;
            if (owner.GetComponents<PlaneController>() != null)
            {
                if(enemyAi) GameManager.Instance.enemiesKilled += 1;
                else GameManager.Instance.friendliesKilled += 1;
            }
                
        }
    }

    public void Crash()
    {
        rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        if (rb.angularVelocity.y <= 0) { rb.AddTorque(new Vector3(0, 100 * -responsiveness, 0)); }
        else { rb.AddTorque(new Vector3( 0, 100 * responsiveness, 0)); }
    }

    public void Explode()
    {
        if (enemyAi) GameManager.Instance.enemies--;
        else GameManager.Instance.friendlies--;
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 1);
        Destroy(transform.gameObject);
    }

    private bool IsEnemy(string tag)
    {
        if (enemyAi && tag == "Corsair")
            return true;
        else if (!enemyAi && tag == "Zero")
            return true;
        else return false;
    }

}
