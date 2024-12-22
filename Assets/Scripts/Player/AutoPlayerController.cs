using UnityEngine;

public class AutoPlayerController : MonoBehaviour
{
    [SerializeField] float lerpSpeed;
    [SerializeField] float speedCounter;
    [SerializeField] float moveSpeed;
    [SerializeField] float playerMoveSpeedMax = 20f;
    [SerializeField] ParticleSystem particleUpgrade; 
    int teaseHash = Animator.StringToHash("Tease");
    Animator playerAnimator;
    Transform playerTransform;
    private void Start() {
        playerTransform = transform;
        playerAnimator = transform.GetComponentInChildren<Animator>();
    }
    void Update()
    {
        HandleMovement();
    }
    void HandleMovement()
    {
        speedCounter = Mathf.Lerp(speedCounter, moveSpeed, Time.deltaTime * lerpSpeed);
        moveSpeed = Mathf.Lerp(moveSpeed, 0, Time.deltaTime * lerpSpeed);
        playerTransform.Translate(Vector3.forward * speedCounter * Time.deltaTime);
        playerAnimator.SetFloat("Movement Multiplier", (speedCounter / 200f));
    }
    public void HandleTapEnergy()
    {
        moveSpeed = Mathf.Clamp(moveSpeed + 1f, 0, playerMoveSpeedMax);
    }
    public void HandleUpgrade()
    {
        particleUpgrade.Play();
        if (teaseHash != playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            playerAnimator.Play("Tease");
            
        }

    }
}
