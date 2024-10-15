using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour {
    public float liftForce = 3500f, descendForce = 3500f, floatForce = 200f, rotationSpeed = 50f;
    public float maxXRotation = 30f, maxZRotation = 45f, forwardSpeed = 4000f;
    public float spinUpTime = 3f, maxRotorSpeed = 1000f, pitchSpeed = 1f, rollSpeed = 1f;
    public float crashSpeedThreshold = 5f;
    public float damageSpeedThreshold = 2f;
    public float skiDamageSpeedThreshold = 3f;
    public Transform mainRotor, tailRotor;

    private Rigidbody rb;
    private float xRotation, zRotation, yRotation, spinUpTimer, currentRotorSpeed;
    private bool isSpinningUp, isSpinningDown, isGrounded;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        AssignCollisionHandlers();
        rb.isKinematic = false; 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinningDown && currentRotorSpeed < maxRotorSpeed) {
            isSpinningUp = true;
            spinUpTimer = spinUpTime;
            currentRotorSpeed = 0f;
        }
        if (Input.GetKeyDown(KeyCode.X) && currentRotorSpeed > 0f && !isSpinningUp && isGrounded)
            isSpinningDown = true;
        if (isSpinningUp) {
            spinUpTimer -= Time.deltaTime;
            currentRotorSpeed = Mathf.Lerp(0f, maxRotorSpeed, 1 - (spinUpTimer / spinUpTime));
            if (spinUpTimer <= 0f) {
                isSpinningUp = false;
                currentRotorSpeed = maxRotorSpeed;
            }
        }
        if (isSpinningDown) {
            currentRotorSpeed -= (maxRotorSpeed / spinUpTime) * Time.deltaTime;
            if (currentRotorSpeed < 0.5f) {
                currentRotorSpeed = 0f;
                isSpinningDown = false;
            }
        }
        if (isSpinningUp || isSpinningDown || currentRotorSpeed >= maxRotorSpeed) {
            mainRotor.Rotate(Vector3.up, currentRotorSpeed * Time.deltaTime);
            tailRotor.Rotate(Vector3.right, currentRotorSpeed * Time.deltaTime);
        }
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
    }

    void FixedUpdate() {
        if (currentRotorSpeed >= maxRotorSpeed) {
            rb.AddForce(Input.GetKey(KeyCode.W) ? Vector3.up * liftForce : Input.GetKey(KeyCode.S) ? Vector3.down * descendForce : Vector3.down * floatForce);
            float forwardMovement = Input.GetKey(KeyCode.UpArrow) ? forwardSpeed : Input.GetKey(KeyCode.DownArrow) ? forwardSpeed / 2 : 0f;
            if (forwardMovement != 0f)
                rb.AddForce((Input.GetKey(KeyCode.UpArrow) ? 1 : -1) * transform.forward * forwardMovement);
            xRotation = Mathf.Lerp(xRotation, Input.GetKey(KeyCode.UpArrow) ? maxXRotation : Input.GetKey(KeyCode.DownArrow) ? -maxXRotation : 0f, Time.deltaTime * pitchSpeed);
            yRotation += Input.GetKey(KeyCode.A) ? -rotationSpeed * Time.deltaTime : Input.GetKey(KeyCode.D) ? rotationSpeed * Time.deltaTime : 0f;
            zRotation += Input.GetKey(KeyCode.LeftArrow) ? rotationSpeed * Time.deltaTime : Input.GetKey(KeyCode.RightArrow) ? -rotationSpeed * Time.deltaTime : -zRotation * Time.deltaTime * rollSpeed;
            zRotation = Mathf.Clamp(zRotation, -maxZRotation, maxZRotation);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                rb.AddForce((Input.GetKey(KeyCode.LeftArrow) ? -1 : 1) * transform.right * forwardSpeed / 2);
        }
    }

    void AssignCollisionHandlers() {
        List<MeshCollider> childColliders = new List<MeshCollider>();
        foreach (Transform child in GetComponentsInChildren<Transform>()) {
            MeshCollider triggerCollider = child.GetComponent<MeshCollider>();
            if (triggerCollider != null) {
                triggerCollider.convex = true;
                triggerCollider.isTrigger = true;
                childColliders.Add(triggerCollider);
                MeshCollider nonTriggerCollider = child.gameObject.AddComponent<MeshCollider>();
                nonTriggerCollider.sharedMesh = triggerCollider.sharedMesh;
                nonTriggerCollider.convex = true;
                nonTriggerCollider.isTrigger = false;
                childColliders.Add(nonTriggerCollider);
                HelicopterPartCollisionHandler collisionHandler = child.gameObject.AddComponent<HelicopterPartCollisionHandler>();
                collisionHandler.helicopterController = this;
                AssignPartType(collisionHandler, child);
            }
        }
        for (int i = 0; i < childColliders.Count; i++)
            for (int j = i + 1; j < childColliders.Count; j++)
                Physics.IgnoreCollision(childColliders[i], childColliders[j]);
    }

    void AssignPartType(HelicopterPartCollisionHandler handler, Transform child) {
        if (child.name.ToLower().Contains("rotor"))
            handler.partType = HelicopterPartCollisionHandler.PartType.Rotor;
        else if (child.name.ToLower().Contains("body"))
            handler.partType = HelicopterPartCollisionHandler.PartType.Body;
        else if (child.name.ToLower().Contains("ski l"))
            handler.partType = HelicopterPartCollisionHandler.PartType.SkiL;
        else if (child.name.ToLower().Contains("ski r"))
            handler.partType = HelicopterPartCollisionHandler.PartType.SkiR;
    }

    public void HandlePartCollision(HelicopterPartCollisionHandler.PartType partType, Collider collision) {
        switch (partType) {
            case HelicopterPartCollisionHandler.PartType.Rotor:
                HandleRotorCollision();
                break;
            case HelicopterPartCollisionHandler.PartType.Body:
                HandleBodyCollision(collision);
                break;
            case HelicopterPartCollisionHandler.PartType.SkiL:
            case HelicopterPartCollisionHandler.PartType.SkiR:
                HandleSkiCollision(partType, collision);
                break;
        }
    }

    void HandleRotorCollision() {
        Debug.Log("Rotor crash!");
        HandleCrash();
    }

    void HandleBodyCollision(Collider collision) {
        float impactSpeed = rb.velocity.magnitude;

        if (impactSpeed > crashSpeedThreshold) {
            Debug.Log("Body Crash");
            HandleCrash();
        }
        else {
            Debug.Log("Body Minor damage.");
            HandleMinorDamage();
        }
    }

    void HandleSkiCollision(HelicopterPartCollisionHandler.PartType skiPart, Collider collision) {
        RaycastHit hit;
        if (Physics.Raycast(collision.transform.position, -Vector3.up, out hit)) {
            Vector3 collisionNormal = hit.normal;
            if (Vector3.Dot(collisionNormal, Vector3.up) > 0.7f) {
                Debug.Log("Landing detected.");
                isGrounded = true;
            }
            else if (rb.velocity.magnitude > skiDamageSpeedThreshold) {
                Debug.Log($"{skiPart} collision");
                HandleSkiDamage(skiPart);
            }
        }
    }

    void HandleCrash() {
        Debug.Log("Crash!");
    }

    void HandleMinorDamage() {
        Debug.Log("Minor damage!");
    }

    void HandleSkiDamage(HelicopterPartCollisionHandler.PartType skiPart) {
        Debug.Log($"{skiPart} has been damaged or detached!");
    }
}