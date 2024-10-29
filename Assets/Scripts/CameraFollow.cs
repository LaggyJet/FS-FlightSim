using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance { get; private set; }
    public Transform target;
    public Vector3 offset = new Vector3(-0.275878906f, 7.25f, -7.35939026f);
    public float mouseSensitivity = 100f;
    private float pitch = 0f; 
    private float yaw = 0f;
    public Vector2 lookPos;

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

    void Update() { if (Mouse.current != null) lookPos += Mouse.current.delta.ReadValue(); }

    void LateUpdate() {
        if (target != null) {
            float mouseX = lookPos.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookPos.y * mouseSensitivity * Time.deltaTime;
            yaw += mouseX;
            pitch += mouseY;
            pitch = Mathf.Clamp(pitch, -85f, 85f); 
            float targetYRotation = target.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(pitch, targetYRotation + yaw, 0f);
            transform.position = target.position + rotation * offset;
            transform.LookAt(target.position);
            lookPos = Vector2.zero;
        }
    }
}