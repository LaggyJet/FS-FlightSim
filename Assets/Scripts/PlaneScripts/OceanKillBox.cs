using UnityEngine;

public class OceanKillBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        PlaneAi temp = collision.gameObject.GetComponent<PlaneAi>();
        if(temp != null) temp.Explode();
        collision.gameObject.GetComponent<PlaneController>()?.Explode();
    }
}
