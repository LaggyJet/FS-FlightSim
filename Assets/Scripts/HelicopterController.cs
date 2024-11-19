using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterController : MonoBehaviour {
    [SerializeField] float liftForce = 500f, descendForce = 500f, floatForce = 40f, rotationSpeed = 50f;
    [SerializeField] float maxXRotation = 30f, maxZRotation = 45f, forwardSpeed = 800f;
    [SerializeField] float spinUpTime = 3f, maxRotorSpeed = 1000f, pitchSpeed = 1f, rollSpeed = 1f;
    [SerializeField] float crashSpeedThreshold = 1f;
    [SerializeField] Transform mainRotor, tailRotor;
    [SerializeField] GameObject destroyedHeliPrefab;
    [SerializeField] LayerMask helipadMask;
    Rigidbody rb;
    float xRotation, zRotation, yRotation, spinUpTimer, currentRotorSpeed;
    bool isSpinningUp, isSpinningDown, isGrounded;
    bool isMovingUp = false, isMovingDown = false, isMovingLeft = false, isMovingRight = false, isMovingForward = false, isMovingBackward = false, isLeaningLeft = false, isLeaningRight = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        AssignCollisionHandlers();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update() {
        if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame) {
            isMovingUp = Input.GetAxis("FwdBck") > 0.5f;
            isMovingDown = Input.GetAxis("FwdBck") < -0.5f;
            isMovingLeft = Input.GetAxis("Yaw") < -0.2f;
            isMovingRight = Input.GetAxis("Yaw") > 0.2f;
            isMovingForward = Input.GetAxis("Vertical") > 0.2f;
            isMovingBackward = Input.GetAxis("Vertical") < -0.2f;
            isLeaningLeft = Input.GetAxis("Horizontal") < -0.2f;
            isLeaningRight = Input.GetAxis("Horizontal") > 0.2f;
            if (Input.GetKeyDown(KeyCode.JoystickButton4))
                OnTurnOn();
            if (Input.GetKeyDown(KeyCode.JoystickButton5))
                OnShutdown();
            if (Input.GetKeyDown(KeyCode.JoystickButton10))
                OnPauseResume();
        }

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
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 2, helipadMask))
            PlatformController.singleton.Heave = Mathf.Lerp(PlatformController.singleton.Heave, ScoreChecker.Map((transform.position - hit.point).magnitude, 0.18f, 2.0f, -8f, 16f), 0.03f);
        else
            PlatformController.singleton.Heave = 0f;
        PlatformController.singleton.Pitch = xRotation * 0.4f;
        PlatformController.singleton.Yaw = Mathf.Lerp(PlatformController.singleton.Yaw, isMovingLeft ? -12 : isMovingRight ? 12 : 0, 0.05f);
        PlatformController.singleton.Roll = -zRotation * 0.3f;
    }

    void OnUp(InputValue value) { isMovingUp = value.isPressed; isGrounded = false; }

    void OnDown(InputValue value) { isMovingDown = value.isPressed; }

    void OnLeft(InputValue value) { isMovingLeft = value.isPressed; }

    void OnRight(InputValue value) { isMovingRight = value.isPressed; }

    void OnForward(InputValue value) { isMovingForward = value.isPressed; }

    void OnBackward(InputValue value) { isMovingBackward = value.isPressed; }

    void OnLeanLeft(InputValue value) { isLeaningLeft = value.isPressed; }

    void OnLeanRight(InputValue value) { isLeaningRight = value.isPressed; }

    void OnTurnOn() {
        GameManager.Instance.startedGame = true;
        if (!isSpinningDown && currentRotorSpeed < maxRotorSpeed && !isSpinningUp) {
            isSpinningUp = true;
            spinUpTimer = spinUpTime;
            currentRotorSpeed = 0f;
        }
    }

    void OnShutdown() { 
        if (currentRotorSpeed > 0f && !isSpinningUp && isGrounded) { 
            isSpinningDown = true;
            if (GameManager.Instance.finishedObjectives)
                GameManager.Instance.CallWinGame();
        } 
    }

    void OnPauseResume() { (GameManager.Instance.isPaused ? (Action)GameManager.Instance.ResumeGame : GameManager.Instance.PauseGame)(); }

    void FixedUpdate() {
        if (currentRotorSpeed >= maxRotorSpeed) {
            rb.AddForce((isMovingUp ? Vector3.up : isMovingDown ? Vector3.down : Vector3.down) * (isMovingUp ? liftForce : isMovingDown ? descendForce : floatForce));
            Vector3 fwd = transform.forward;
            fwd.y = 0;
            rb.AddForce((isMovingForward ? fwd : isMovingBackward ? -fwd * 0.5f : Vector3.zero) * forwardSpeed);
            xRotation = Mathf.Lerp(xRotation, isMovingForward ? maxXRotation : isMovingBackward ? -maxXRotation : 0f, Time.fixedDeltaTime * pitchSpeed);
            yRotation += (isMovingLeft ? -rotationSpeed : isMovingRight ? rotationSpeed : 0f) * Time.fixedDeltaTime;
            zRotation += (isLeaningLeft ? rotationSpeed : isLeaningRight ? -rotationSpeed : -zRotation * rollSpeed) * Time.fixedDeltaTime;
            zRotation = Mathf.Clamp(zRotation, -maxZRotation, maxZRotation);
            if (isLeaningLeft || isLeaningRight) {
                fwd = transform.right;
                fwd.y = 0;
                rb.AddForce((isLeaningLeft ? -1 : 1) * forwardSpeed * fwd / 1.5f);
            }
        }
    }

    void AssignCollisionHandlers() {
        List<MeshCollider> childColliders = new();
        foreach (Transform child in GetComponentsInChildren<Transform>()) {
            if (child.TryGetComponent<MeshCollider>(out var triggerCollider)) {
                if (child.name.ToLower() == "rotor" || child.name.ToLower() == "body") {
                    triggerCollider.convex = true;
                    triggerCollider.isTrigger = true;
                    childColliders.Add(triggerCollider);
                    MeshCollider nonTriggerCollider = child.gameObject.AddComponent<MeshCollider>();
                    nonTriggerCollider.sharedMesh = triggerCollider.sharedMesh;
                    nonTriggerCollider.convex = true;
                    nonTriggerCollider.isTrigger = false;
                    childColliders.Add(nonTriggerCollider);
                }
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
        GameObject destroyedHeli = Instantiate(destroyedHeliPrefab, transform.position, transform.rotation);
        CameraFollow.Instance.ChangeTarget(destroyedHeli.transform);
        ObjectHelper.Explode(destroyedHeli);
        GameManager.Instance.CallLoseGame();
        Destroy(gameObject);
    }

    public void HandlePartCollision(HelicopterPartCollisionHandler.PartType partType, Collider collision) {
        switch (partType) {
            case HelicopterPartCollisionHandler.PartType.Rotor:
                Explode();
                break;
            case HelicopterPartCollisionHandler.PartType.Body:
                if (rb.velocity.magnitude > crashSpeedThreshold) 
                    Explode();
                break;
            case HelicopterPartCollisionHandler.PartType.SkiL:
            case HelicopterPartCollisionHandler.PartType.SkiR:
                HandleSkiCollision(partType, collision);
                break;
        }
    }

    public bool skiRLanded = false;
    public bool skiLLanded = false;

    void HandleSkiCollision(HelicopterPartCollisionHandler.PartType skiPart, Collider collision) {
        if (!collision.CompareTag("Helipad")) 
            return;
        if (skiPart == HelicopterPartCollisionHandler.PartType.SkiR) 
            skiRLanded = true;
        else if (skiPart == HelicopterPartCollisionHandler.PartType.SkiL) 
            skiLLanded = true;
        if (skiRLanded && skiLLanded) {
            bool wasCompleted = false;
            int objectIndex = -1;
            var gObject = GameManager.Instance.objectivesCompleted;
            for (int i = 0; i < gObject.Length; i++) {
                if (collision.gameObject == gObject[i].Item1) {
                    objectIndex = i;
                    if (gObject[i].Item2)
                        wasCompleted = true;
                }
            }
            if (!wasCompleted) {
                UIUpdater.Instance.UpdateCurrentObjectiveScore();
                GameManager.Instance.objectivesCompleted[objectIndex] = Tuple.Create(gObject[objectIndex].Item1, true);
                GameManager.Instance.objectiveAccuries[objectIndex] = ScoreChecker.GetHeliPadAccuracy(transform.position.x - collision.transform.position.x, transform.position.z - collision.transform.position.z);
                GameManager.Instance.accuracy = ScoreChecker.GetOverallAccuracy(GameManager.Instance.objectiveAccuries);
                
            }
            isGrounded = true;
        }
    }
}