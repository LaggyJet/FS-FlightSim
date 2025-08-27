using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class PlaneController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    //Hidden SerializeFields
    [SerializeField, HideInDebugUI] GameObject explosion;
    [SerializeField, HideInDebugUI] GameObject[] animatedParts; //prop, elevator, right aeileron, left aieleron, rudder
    [HideInDebugUI] AnimationInterface[] interfaces;

    [Header("Plane variables")]
    [SerializeField] float throttleIncrement = 0.01f;
    [SerializeField] float thrustMax = 746.0f;
    [SerializeField] float responsiveness = 100.0f;
    [SerializeField] float area = 26;
    [SerializeField] float gravity = 65;
    [SerializeField] float liftCoefficient = 0.8f;
    [SerializeField] float dragCoefficient = 0.04f;
    [SerializeField] int ammoMain = 400;
    [SerializeField] int ammoSecondary = 300;
    [SerializeField] AudioSource gunSound;

    //for use in functions
    bool running = false;
    bool startedPlaying = false;
    bool initialCall = true;
    bool firing;
    float throttle;
    float idle = 1f;
    float roll;
    float pitch;
    float yaw;
    float responseModifier { get { return (rb.mass / 10.0f) * responsiveness; } }
    GunSystem weapons;

    //debug variables
    [SerializeField, HideInDebugUI] bool drawlines;
    

    private void Awake()
    {
        //sets all our variables
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<GunSystem>();
        responsiveness = Mathf.Pow(responsiveness, 3);
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
        if(Input.GetButtonDown("Fire1"))
        {
            gunSound.Play();
            firing = true;
        }
        if(Input.GetButtonUp("Fire1"))
        {
            gunSound.Pause();
            firing = false;
        }
        pitch = Input.GetAxis("Pitch");
        HandleAnimations(1, Numbers.Map(pitch, -1, 1, 0, 1));
        roll = Input.GetAxis("Roll");
        HandleAnimations(3, Numbers.Map(roll, -1, 1, 0, 1));
        HandleAnimations(2, Numbers.Map(roll, -1, 1, 0, 1));
        yaw = Input.GetAxis("Yaw");
        HandleAnimations(4, Numbers.Map(yaw, -1, 1, 0, 1));


        PlatformController.singleton.Pitch = pitch * 8;
        PlatformController.singleton.Roll = -roll * 8;
        PlatformController.singleton.Yaw = yaw * 8;



        if (Input.GetButtonDown("Ignition")) 
        {
            if(running)
            {
                running = HandleAnimations(0, false);
                throttle = HandleAnimations(0, 0);
            }
            else
            {
                if (Input.GetAxis("Throttle") == -1)
                {
                    running = HandleAnimations(0, true);
                    throttle = HandleAnimations(0, idle);
                }
            }
        }

        float throttleLevel = Numbers.Map(Input.GetAxis("Throttle"), -1, 1, 0, 100);
        print(Input.GetAxis("Throttle"));

        if (running) { throttle = HandleAnimations(0, throttleLevel); }
        else { throttle = HandleAnimations(0, 0); }
    }

    
    private void Update()
    {
        if (StartedPlaying() && initialCall && GameManager.Instance.currentManager is PlaneGameManager planeManager) { planeManager.runTimer = true; initialCall = false; }
        if(firing) {  weapons.Fire(this.gameObject); ammoMain--; ammoSecondary--; }
            
        HandleInputs();
        if(drawlines) Debug.DrawLine(rb.position, rb.position + rb.linearVelocity, Color.yellow);
    }

    private void FixedUpdate()
    { 
        rb.AddForce(CalculateForce(rb.linearVelocity, rb.angularVelocity, 1.2f));
        rb.AddTorque(CalculateTorque());
    }

    private Vector3 CalculateForce(Vector3 velocity, Vector3 angularVelocity, float airDensity)
    {
        //this is the vector we will be adding forces to in order to return at the end of the function
        Vector3 force = new Vector3();

        //air pressure dynamically changing with height to use in the lift and drag formulas
        float pressure = (airDensity * rb.transform.forward.sqrMagnitude) / 2;

        //our thrust, lift, drag, and weight formulas
        Vector3 thrust = rb.transform.forward * (thrustMax * throttle);
        Vector3 lift = rb.transform.up * liftCoefficient * pressure * area;
        Vector3 drag = rb.transform.forward * dragCoefficient * pressure * area;

        force = lift + thrust - drag;

        Vector3 weight = Vector3.down * gravity;
        weight /= (thrust.magnitude + lift.magnitude);
        rb.AddForce(weight, ForceMode.Acceleration);

        return force;
    }

    public void Explode()
    {
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 1);
        GameObject.Find("3rd_Person_Camera").transform.parent = null;
        Destroy(transform.gameObject);
        UI.Instance.Lose();
    }

    private Vector3 CalculateTorque()
    {
        Vector3 torque = new Vector3();

        torque += transform.right * pitch * responsiveness;
        torque += transform.forward * roll * responsiveness;
        torque += transform.up* yaw * responsiveness;

        return torque;
    } 

    private bool StartedPlaying() {
        bool returnVal = false;
        switch(GameManager.Instance.selectedGameMode.category) {
            case GameMode.Category.Plane:
                switch ((GameMode.PlaneMode)GameManager.Instance.selectedGameMode.mode) {
                    case GameMode.PlaneMode.DogFight:
                    if (running) returnVal = true;
                    break;

                }
                break;
        }
        return returnVal;
    }

    private float HandleAnimations(int part, float val)
    {
        interfaces[part].SetValue(val);
        return val;
    }

    private bool HandleAnimations(int part, bool val)
    {
        interfaces[part].SetBool(val);
        return val;
    }


}
