using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("Plane variables")]
    [Tooltip("How Much the Throttle is Increased or Decreased by Each Input")]
    [SerializeField] public float throttleIncrement = 0.5f;
    [Tooltip("Maximum Engine Thrust at 100% Throttle")]
    [SerializeField] public float thrustMax = 550.0f;
    [Tooltip("How Responsive the Plane is When Maneuvering")]
    [SerializeField] public float responsiveness = 10.0f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;

    //This is makes our responsiveness of the plane be scaled with its mass
    private float responseModifier { get { return (rb.mass / 10.0f) * responsiveness; } }

    private float idle = 1f;
    private bool running = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void HandleInputs()
    {
        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if(Input.GetKey(KeyCode.BackQuote)) 
        {
            running = !running;
            if(running) { throttle = idle;}
            else { throttle = 0; }
        }
        //throttle up
        if (Input.GetKey(KeyCode.Space)) { throttle += (throttleIncrement * throttle); }
        //throttle down
        else if (Input.GetKey(KeyCode.LeftShift)) { throttle -= (throttleIncrement * throttle); }
        //break(WIP)
        //else if(Input.GetKey(KeyCode.RightShift)) { throttle -= throttleIncrement * 3; }

        if (running) { throttle = Mathf.Clamp(throttle, idle, 100.0f); }
        else { throttle = 0; }




    }

    private void Update()
    {
        HandleInputs();


    }

    private void FixedUpdate()
    {
        if(!running) { rb.AddForce(transform.up * -13.8f); }
        rb.AddForce(transform.forward * thrustMax * throttle);
        rb.AddTorque(transform.up * yaw * responseModifier * 10);
        rb.AddTorque(transform.right * pitch * responseModifier * 10);
        rb.AddTorque(transform.forward * roll * responseModifier);
    }

}
