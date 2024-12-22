using UnityEngine;

public class ProbObstacle : MonoBehaviour
{
    [SerializeField]float pushForce = 40f;
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            Vector3 directionPositionPush = other.transform.position - transform.position;
            other.transform.Translate(directionPositionPush.normalized * pushForce * Time.deltaTime);
        }
    }
}
