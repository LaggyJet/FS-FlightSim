using UnityEngine;

public class BoundingBox : MonoBehaviour {
    [SerializeField] Transform oppositeWall;
    enum CurrentWall { Left, Right, Front, Back, Roof };
    [SerializeField] CurrentWall curWall;
    bool missileLaunched = false;

    void OnTriggerEnter(Collider other) {
        if (curWall != CurrentWall.Roof && !other.CompareTag("Missile")) {
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
        else if (!missileLaunched) {
            missileLaunched = true;
            MissileLauncher.Instance.LaunchMissile(other.transform.root.gameObject);
        }
    }

    void OnTriggerExit(Collider other) { 
        if (curWall == CurrentWall.Roof && other.transform.root.CompareTag("Heli")) {
            MissileLauncher.Instance.CancelMissile(other.transform.root.gameObject);
            missileLaunched = false;
        }
    }
}
