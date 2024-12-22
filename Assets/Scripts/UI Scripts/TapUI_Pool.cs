using UnityEngine;

public class TapUI_Pool : MonoBehaviour
{
    private Animator tapUIAnimator;
    private void OnEnable() {
        tapUIAnimator = GetComponent<Animator>();
        tapUIAnimator.Play("PoolUI");
    }
    private void Update() {
        AnimatorStateInfo stateInfo = tapUIAnimator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("PoolUI"))
        {
            gameObject.SetActive(false);
        }
    }
}
