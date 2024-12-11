using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class PlaneController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject[] animatedParts; //prop, elevator, right aeileron, left aieleron, rudder
    AnimationInterface[] interfaces;

    [Header("Plane variables")]
    [Tooltip("How Much the Throttle is Increased or Decreased by Each Input")]
    [SerializeField] public float throttleIncrement = 0.01f;
    [Tooltip("Maximum Engine Thrust at 100% Throttle")]
    [SerializeField] public float thrustMax = 746.0f;
    [Tooltip("How Responsive the Plane is When Maneuvering")]
    [SerializeField] public float responsiveness = 100.0f;

    float throttle;
    float roll;
    float pitch;
    float yaw;
    float liftCoefficient = 0.8f;
    float dragCoefficient = 0.04f;
    float area = 26;

    GunSystem weapons;
    bool firing;

    //This is makes our responsiveness of the plane be scaled with its mass
    float responseModifier { get { return (rb.mass / 10.0f) * responsiveness; } }

    float idle = 1f;
    bool running = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<GunSystem>();
        interfaces = new AnimationInterface[animatedParts.Length];
        int temp = 0;
        foreach(var part in animatedParts)
        {
            interfaces[temp] = part.GetComponent<AnimationInterface>();
            temp++;
        }
    }

    private void HandleInputs()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            firing = true;
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            firing = false;
        }
        pitch = HandleAnimations(1, Input.GetAxis("Pitch"), false);
        roll = HandleAnimations(3, Input.GetAxis("Roll"), false);
        HandleAnimations(2, Input.GetAxis("Roll"), false);
        yaw = HandleAnimations(4, Input.GetAxis("Yaw"), false);

        if(Input.GetKeyDown(KeyCode.BackQuote)) 
        {
            running = HandleAnimations(0, !running);
            if(running) { throttle = HandleAnimations(0, idle, false);}
            else { throttle = HandleAnimations(0, 0, false); }
        }
        //throttle up
        if (Input.GetKey(KeyCode.Space)) { throttle = HandleAnimations(0, throttle + (Mathf.Pow(throttleIncrement, throttleIncrement / throttle)), true); }
        //throttle down
        else if (Input.GetKey(KeyCode.LeftShift)) { throttle = HandleAnimations(0, throttle - (Mathf.Pow(throttleIncrement, throttle)), true); }
        //break(WIP)
        //else if(Input.GetKey(KeyCode.RightShift)) { throttle -= throttleIncrement * 3; }

        if (running) { throttle = HandleAnimations(0, Mathf.Clamp(throttle, idle, 100.0f), true); }
        else { throttle = HandleAnimations(0, 0, false); }

    }

    

    private void Update()
    {
        if(firing)
            weapons.fire();
        HandleInputs();
        Debug.DrawLine(rb.position, rb.position + rb.linearVelocity, Color.yellow);
        //code for finding angle of attack to use for stall and weight
        //Vector3.SignedAngle(rb.transform.forward, rb.linearVelocity, Vector3.right);
    }

    private void FixedUpdate()
    { 
        rb.AddForce(CalculateForce(rb.linearVelocity, rb.angularVelocity, 1.2f));
        rb.AddTorque(CalculateTorque());

        //float v = transform.InverseTransformDirection(rb.linearVelocity).z  * 2.2f;
        //print(force);
        //print(v);
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

    private float HandleAnimations(int part, float val, bool large)
    {
        interfaces[part].SetValue(val, large);
        return val;
    }

    private bool HandleAnimations(int part, bool val)
    {
        interfaces[part].SetBool(val);
        return val;
    }

}
