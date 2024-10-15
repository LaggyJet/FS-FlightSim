using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public Vector3 offset = new Vector3(-0.275878906f, 7.25f, -7.35939026f);
    public float mouseSensitivity = 100f;
    private float pitch = 0f; 
    private float yaw = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate() {
        if (target != null) {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            yaw += mouseX;
            pitch += mouseY;
            pitch = Mathf.Clamp(pitch, -85f, 85f); 
            float targetYRotation = target.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(pitch, targetYRotation + yaw, 0f);
            transform.position = target.position + rotation * offset;
            transform.LookAt(target.position);
        }
    }
}