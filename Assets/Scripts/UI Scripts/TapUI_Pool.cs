using UnityEngine;

public class TapUI_Pool : MonoBehaviour
{
    private Animator animator;
    private void OnEnable() {
        animator = GetComponent<Animator>();
        animator.Play("PoolUI");
    }
    private void Update() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("PoolUI"))
        {
            gameObject.SetActive(false);
        }
    }
}
