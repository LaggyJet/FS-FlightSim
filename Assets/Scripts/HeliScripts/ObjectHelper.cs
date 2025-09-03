using System.Collections.Generic;
using UnityEngine;

public static class ObjectHelper {
    static public void ExplodeObject(GameObject gameObject, float force) {
        if (!gameObject)
            return;

        MeshFilter[] mf = gameObject.GetComponentsInChildren<MeshFilter>();
        List<Rigidbody> bodies = new(mf.Length);
        foreach (MeshFilter child in mf) {
            MeshCollider mc = child.GetComponent<MeshCollider>();
            if (!mc)
                mc = child.gameObject.AddComponent<MeshCollider>();
            mc.convex = true;

            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (!rb)
                rb = child.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
            
            bodies.Add(rb);
        }

        foreach (Rigidbody body in bodies)
            body.AddForce(force * (Random.onUnitSphere + Vector3.up), ForceMode.VelocityChange);
    }
}
