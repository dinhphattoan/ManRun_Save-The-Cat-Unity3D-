using UnityEngine;

public class ProbObstacle : MonoBehaviour
{
    [SerializeField] float pushForce = 40f;
    private void OnTriggerStay(Collider triggeredCollider)
    {
        if (triggeredCollider.gameObject.tag == "Player")
        {
            Vector3 directionPositionPush = triggeredCollider.transform.position - transform.position;
            triggeredCollider.transform.Translate(directionPositionPush.normalized * pushForce * Time.deltaTime);
        }
    }
}
