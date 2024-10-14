using UnityEngine;

public class HelicopterController : MonoBehaviour {
    public float liftForce = 3500f, descendForce = 3500f, floatForce = 200f, rotationSpeed = 50f, maxXRotation = 30f, maxZRotation = 45f, forwardSpeed = 4000f, spinUpTime = 3f, maxRotorSpeed = 1000f, pitchSpeed = 1f, rollSpeed = 1f;
    public Transform mainRotor, tailRotor;
    public GameObject skiL, skiR;

    private Rigidbody rb;
    private float xRotation, zRotation, yRotation, spinUpTimer, currentRotorSpeed;
    private bool isSpinningUp, isSpinningDown;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinningDown && currentRotorSpeed < maxRotorSpeed) {
            isSpinningUp = true;
            spinUpTimer = spinUpTime;
            currentRotorSpeed = 0f;
        }
        if (Input.GetKeyDown(KeyCode.X) && currentRotorSpeed > 0f && !isSpinningUp)
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
        bool isGrounded = Physics.Raycast(skiL.transform.position, Vector3.down, 0.1f) || Physics.Raycast(skiR.transform.position, Vector3.down, 0.1f);
        if (!isGrounded)
            HandleCrash();
        if (currentRotorSpeed == maxRotorSpeed) {
            Vector3 lift = Input.GetKey(KeyCode.W) ? Vector3.up * liftForce : Input.GetKey(KeyCode.S) ? Vector3.down * descendForce : Vector3.down * floatForce;
            rb.AddForce(lift);
            float forwardSpeed = Input.GetKey(KeyCode.UpArrow) ? this.forwardSpeed : Input.GetKey(KeyCode.DownArrow) ? this.forwardSpeed / 2 : 0f;
            if (forwardSpeed != 0f)
                rb.AddForce((Input.GetKey(KeyCode.UpArrow) ? 1 : -1) * transform.forward * forwardSpeed);
            xRotation = Mathf.Lerp(xRotation, Input.GetKey(KeyCode.UpArrow) ? maxXRotation : Input.GetKey(KeyCode.DownArrow) ? -maxXRotation : 0f, Time.deltaTime * pitchSpeed);
            yRotation += Input.GetKey(KeyCode.A) ? -rotationSpeed * Time.deltaTime : Input.GetKey(KeyCode.D) ? rotationSpeed * Time.deltaTime : 0f;
            zRotation += Input.GetKey(KeyCode.LeftArrow) ? rotationSpeed * Time.deltaTime : Input.GetKey(KeyCode.RightArrow) ? -rotationSpeed * Time.deltaTime : -zRotation * Time.deltaTime * rollSpeed;
            zRotation = Mathf.Clamp(zRotation, -maxZRotation, maxZRotation);
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                rb.AddForce((Input.GetKey(KeyCode.LeftArrow) ? -1 : 1) * transform.right * this.forwardSpeed / 2);
        }
    }

    void HandleCrash() {
        Debug.Log("Crash detected!");
    }
}