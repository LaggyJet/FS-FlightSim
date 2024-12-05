using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class PlaneController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("Plane variables")]
    [Tooltip("How Much the Throttle is Increased or Decreased by Each Input")]
    [SerializeField] public int throttleIncrement = 1;
    [Tooltip("Maximum Engine Thrust at 100% Throttle")]
    [SerializeField] public float thrustMax = 746.0f;
    [Tooltip("How Responsive the Plane is When Maneuvering")]
    [SerializeField] public float responsiveness = 100.0f;

    private float throttle;
    private float roll;
    private float pitch;
    private float yaw;
    float liftCoefficient = 0.8f;
    float dragCoefficient = 0.04f;
    float area = 26;

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

        if(Input.GetKeyDown(KeyCode.BackQuote)) 
        {
            running = !running;
            if(running) { throttle = idle;}
            else { throttle = 0; }
        }
        //throttle up
        if (Input.GetKey(KeyCode.Space)) { throttle += (Mathf.Pow(2f, throttleIncrement)); }
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
        Debug.DrawLine(rb.position, rb.position + rb.linearVelocity, Color.yellow);
        //code for finding angle of attack to use for stall and weight
        //Vector3.SignedAngle(rb.transform.forward, rb.linearVelocity, Vector3.right);
    }

    private void FixedUpdate()
    { 
        rb.AddForce(CalculateForce(rb.linearVelocity, rb.angularVelocity, 1.2f));
        rb.AddTorque(CalculateTorque());

        float v = transform.InverseTransformDirection(rb.linearVelocity).z  * 2.2f;
        //print(force);
        print(v);
    }

    private Vector3 CalculateForce(Vector3 velocity, Vector3 angularVelocity, float airDensity)
    {
        //this is the vector we will be adding forces to in order to return at the end of the function
        Vector3 force = new Vector3();

        //air pressure dynamically changing with height to use in the lift and drag formulas
        float pressure = (airDensity * rb.transform.forward.sqrMagnitude) / 2;

        //our thrust, lift, and drag formulas
        Vector3 thrust = rb.transform.forward * (thrustMax * throttle);
        Vector3 lift = rb.transform.up * liftCoefficient * pressure * area;
        Vector3 drag = rb.transform.forward * dragCoefficient * pressure * area;
        
        force = lift + thrust - drag;

        return force;
    }

    private Vector3 CalculateTorque()
    {
        Vector3 torque = new Vector3();

        torque += transform.right * pitch * responsiveness * responsiveness;
        torque += transform.forward * roll * responsiveness * responsiveness;
        torque += transform.up* yaw *responsiveness * responsiveness;

        return torque;
    }             

    
}
