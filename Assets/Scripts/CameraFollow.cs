using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance { get; private set; }
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 50f;
    Vector2 lookPos;
    float pitch = 0f;
    float yaw = 0f;
    Vector3 initialOffset;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (target != null) {
            initialOffset = Quaternion.Euler(30f, target.eulerAngles.y, 0f) * (target.forward * -0.9f);
            transform.position = target.position + initialOffset;
            transform.LookAt(target.position);
        }
    }

    void Update() {
        if (Mouse.current != null)
            lookPos += Mouse.current.delta.ReadValue();
        if (Gamepad.current != null) {
            Vector2 curval = Gamepad.current.rightStick.ReadValue();
            yaw += curval.x;
            pitch += curval.y;
        }
        if (Joystick.current != null) {
            yaw += Input.GetAxis("HatX");
            pitch += Input.GetAxis("HatY");
        }
    }

    void LateUpdate() {
        if (target != null) {
            yaw += lookPos.x * mouseSensitivity * Time.deltaTime;
            pitch += lookPos.y * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, -30f, 59.99f);
            transform.position = target.position + (Quaternion.Euler(pitch, target.eulerAngles.y + yaw, 0f) * initialOffset);
            transform.LookAt(target.position);
            lookPos = Vector2.zero;
        }
    }

    public void ChangeTarget(Transform newTarget) { target = newTarget; }
}