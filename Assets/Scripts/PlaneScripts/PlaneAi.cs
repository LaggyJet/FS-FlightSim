using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlaneAi : MonoBehaviour
{
    [SerializeField] Transform LOS;
    [SerializeField] LayerMask mask;
    [SerializeField] float hp;
    [SerializeField] float sightDistance = 100f;
    [SerializeField] float speed = 80f;
    [SerializeField] float responsiveness = 15f;
    [SerializeField] float gravity = 100f;
    Rigidbody rb;
    List<GameObject> boids = new List<GameObject>();
    List<Vector3> avoid = new List<Vector3>();
    [SerializeField, HideInDebugUI]bool crashing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        for(int x = 0; x < 100; x++)
        {
            
            if (Physics.Raycast(LOS.position, LOS.forward, out RaycastHit hit, 100, mask))
            {
                Debug.DrawLine(LOS.position, hit.point);
                if (hit.collider.gameObject.GetComponent<PlaneAi>()) boids.Add(hit.collider.gameObject.transform.root.gameObject);
                else avoid.Add(hit.point);
            }
        }
        
    }

    //function for finding any boids we want to follow that are close enough to the plane and starting our movement towards them
    private void FollowBoid()
    {
        if (boids.Count == 0) return;
        //declares a variable for the closest positioned boid we might want to follow and its distance from our plane
        Vector3 closest = Vector3.zero;
        float distance = 5000;
        //loops through our list of boids to see if its one we want to follow and if its closer then our closest
        foreach(GameObject plane in boids)
        {
            if (plane.gameObject.tag == "Foe")
            {
                float temp = Vector3.Distance(this.transform.position, plane.transform.position);
                //if all  requirements are met we make this the new closest and track the distance as this will be used to adjust the plane following and how fast it follows
                //we add our old postion to avoid list as we will now want to avoid it
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

        print(avoid.Count);
    }

    private void AvoidObstacle()
    {
        if(avoid.Count == 0) return;
        //similar but 
        Vector3 safeDirection = Vector3.zero;
        foreach (Vector3 point in avoid)
        {
            float distance = Vector3.Distance(this.transform.position, point);
            safeDirection += (-point) * 1/distance;
        }

        rb.AddTorque(new Vector3(transform.position.x + safeDirection.x * responsiveness, transform.position.y + safeDirection.y * responsiveness, 0));
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0) crashing = true;
    }

    public void Crash()
    {
        Vector3 temp = rb.transform.InverseTransformPoint(new Vector3(rb.transform.TransformPoint(rb.transform.position).x, 0, rb.transform.TransformPoint(rb.transform.position).z));
        Vector3 down = temp * gravity;
        rb.AddForce(down);
        if (rb.angularVelocity.x <= 0) { rb.AddTorque(new Vector3(-responsiveness, 0, 0)); }
        else { rb.AddTorque(new Vector3(responsiveness, 0, 0)); }
    }


    //Rules for BOIDS
    //1.) Avoid Collision With Other Boids
    //2.) Try and Align to Center of Opposing Boids
    //3.) Start Firing if Opposing Boid is In Range
    //4.) Avoid Non-Boid Obstacles
}
