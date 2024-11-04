using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterController : MonoBehaviour {
    [SerializeField] float liftForce = 500f, descendForce = 500f, floatForce = 40f, rotationSpeed = 50f;
    [SerializeField] float maxXRotation = 30f, maxZRotation = 45f, forwardSpeed = 800f;
    [SerializeField] float spinUpTime = 3f, maxRotorSpeed = 1000f, pitchSpeed = 1f, rollSpeed = 1f;
    [SerializeField] float crashSpeedThreshold = 1f;
    [SerializeField] float damageSpeedThreshold = 1f;
    [SerializeField] float skiDamageSpeedThreshold = 1f;
    [SerializeField] Transform mainRotor, tailRotor;
    Rigidbody rb;
    float xRotation, zRotation, yRotation, spinUpTimer, currentRotorSpeed;
    bool isSpinningUp, isSpinningDown, isGrounded;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        AssignCollisionHandlers();
        rb.isKinematic = false; 
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
    }

    void Update() {
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


        //print(Input.GetAxis("Horizontal"));

        //var gamepad = Gamepad.current;
        ////Debug.Log(string.Join("\n", Gamepad.all));
        //if (gamepad == null)
        //{
        //    //Debug.Log("no gamepad");
        //    return; // No gamepad connected.
        //}

        //if (gamepad.rightTrigger.wasPressedThisFrame)
        //{
        //    Debug.Log("Right trigger pressed");
        //}

        //Vector2 move = gamepad.leftStick.ReadValue();
        //{
        //    Debug.Log(move);
        //}
    }

    bool isMovingUp = false;
    bool isMovingDown = false;
    bool isMovingLeft = false;
    bool isMovingRight = false;
    bool isMovingForward = false;
    bool isMovingBackward = false;
    bool isLeaningLeft = false;
    bool isLeaningRight = false;

    void OnUp(InputValue value) { isMovingUp = value.isPressed; isGrounded = false; }

    void OnDown(InputValue value) { isMovingDown = value.isPressed; }

    void OnLeft(InputValue value) { isMovingLeft = value.isPressed; }

    void OnRight(InputValue value) { isMovingRight = value.isPressed; }

    void OnForward(InputValue value) { isMovingForward = value.isPressed; }

    void OnBackward(InputValue value) { isMovingBackward = value.isPressed; }

    void OnLeanLeft(InputValue value) { isLeaningLeft = value.isPressed; }

    void OnLeanRight(InputValue value) { isLeaningRight = value.isPressed; }

    //void OnLook(InputValue value) { CameraFollow.Instance.lookPos += (value.Get<Vector2>() * 3); }

    void OnTurnOn(InputValue value) {
        if (!isSpinningDown && currentRotorSpeed < maxRotorSpeed) {
            isSpinningUp = true;
            spinUpTimer = spinUpTime;
            currentRotorSpeed = 0f;
        }
    }

    void OnShutdown(InputValue value) { if (currentRotorSpeed > 0f && !isSpinningUp && isGrounded) isSpinningDown = true; }


    void OnPauseResume(InputValue value) { (GameManager.Instance.isPaused ? (Action)GameManager.Instance.ResumeGame : GameManager.Instance.PauseGame)(); }

    void FixedUpdate() {
        if (currentRotorSpeed >= maxRotorSpeed) {
            rb.AddForce((isMovingUp ? Vector3.up : isMovingDown ? Vector3.down : Vector3.down) * (isMovingUp ? liftForce : isMovingDown ? descendForce : floatForce));
            rb.AddForce((isMovingForward ? transform.forward : isMovingBackward ? -transform.forward * 0.5f : Vector3.zero) * forwardSpeed);
            xRotation = Mathf.Lerp(xRotation, isMovingForward ? maxXRotation : isMovingBackward ? -maxXRotation : 0f, Time.fixedDeltaTime * pitchSpeed);
            yRotation += (isMovingLeft ? -rotationSpeed : isMovingRight ? rotationSpeed : 0f) * Time.fixedDeltaTime;
            zRotation += (isLeaningLeft ? rotationSpeed : isLeaningRight ? -rotationSpeed : -zRotation * rollSpeed) * Time.fixedDeltaTime;
            zRotation = Mathf.Clamp(zRotation, -maxZRotation, maxZRotation);
            if (isLeaningLeft || isLeaningRight)
                rb.AddForce((isLeaningLeft ? -1 : 1) * transform.right * forwardSpeed / 2);
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

    public void Explode() {
        Debug.Log("Boom");
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