using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance { get; private set; }
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 50f;
    [SerializeField] Vector2 lookPos;

    float pitch = 0f;
    float yaw = 0f;

    void Awake() {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() { 
        if (Mouse.current != null) lookPos += Mouse.current.delta.ReadValue();
        if (Gamepad.current != null) {
            Vector2 curval = Gamepad.current.rightStick.ReadValue();
            yaw += curval.x;
            pitch += curval.y;
        }
    }

    void LateUpdate() {
        if (target != null) {
            yaw += lookPos.x * mouseSensitivity * Time.deltaTime;
            pitch += lookPos.y * mouseSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, 0f, 80f);
            Vector3 v = Quaternion.Euler(pitch, target.eulerAngles.y + yaw, 0f) * new Vector3(1.5f, 1.5f, 1.5f);
            transform.position = target.position + v;
            transform.LookAt(target.position);
            lookPos = Vector2.zero;
        }
    }
}