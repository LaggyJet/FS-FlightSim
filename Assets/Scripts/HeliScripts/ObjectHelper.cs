using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectHelper : MonoBehaviour {
    static public void Explode(GameObject gameObject) {
        MeshFilter[] mf = gameObject.transform.GetComponentsInChildren<MeshFilter>();

        List<Rigidbody> bodies = new();
        for (int i = 0; i < mf.Length; i++) {
            MeshFilter child = mf[i];
            MeshCollider mc = child.AddComponent<MeshCollider>();
            mc.convex = true;
            Rigidbody rb = child.AddComponent<Rigidbody>();
            rb.useGravity = true;
            bodies.Add(rb);
        }

        for (int i = 0; i < bodies.Count; i++)
            bodies[i].AddForce(3 * (Random.onUnitSphere + Vector3.up), ForceMode.VelocityChange);
    }
}
