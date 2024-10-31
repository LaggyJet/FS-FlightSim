using UnityEngine;

public class BoundingBox : MonoBehaviour {
    [SerializeField] Transform oppositeWall;
    enum CurrentWall { Left, Right, Front, Back };
    [SerializeField] CurrentWall curWall;

    void OnTriggerEnter(Collider other) {
        Vector3 helo = other.transform.root.position;
        switch (curWall) {
            case CurrentWall.Left:
                helo.z = oppositeWall.position.z - 10;
                break;
            case CurrentWall.Right:
                helo.z = oppositeWall.position.z + 10;
                break;
            case CurrentWall.Front:
                helo.x = oppositeWall.position.x - 10;
                break;
            case CurrentWall.Back:
                helo.x = oppositeWall.position.x + 10;
                break;
            default:
                break;
        }
        other.transform.root.position = helo;
    }
}
