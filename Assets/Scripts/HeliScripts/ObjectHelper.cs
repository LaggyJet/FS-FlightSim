using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectHelper : MonoBehaviour {
    static public void ExplodeObject(GameObject gameObject, float force) {
        MeshFilter[] mf = gameObject.transform.GetComponentsInChildren<MeshFilter>();

        List<Rigidbody> bodies = new();
        foreach (MeshFilter child in mf) {
            Rigidbody rb = child.AddComponent<Rigidbody>();
            rb.useGravity = true;
            child.AddComponent<MeshCollider>().convex = true;
            bodies.Add(rb);
        }


        foreach (Rigidbody body in bodies)
            body.AddForce(force * (Random.onUnitSphere + Vector3.up), ForceMode.VelocityChange);
    }
}
