using UnityEngine;
using Unity.Mathematics;
using Terresquall;
using GameMechanic;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Transform playerObject;
    private Animator animator;
    private Transform playerTransform;
    [SerializeField] private SO_MechanicSetting playerSetting;
    public SO_MechanicSetting PlayerSetting { get { return playerSetting; } }
    public Vector2 InputDirection => VirtualJoystick.GetAxis();
    public bool isAuto = false;
    public Vector3 autoDirection = Vector3.zero;
    Vector3 targetPosition;
    private void Start()
    {
        animator = playerObject.GetComponent<Animator>();
        playerTransform = transform;
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void HandleMovement()
    {
        if (isAuto || (GameManager.GameStart && InputDirection != Vector2.zero))
        {
            targetPosition = new Vector3(InputDirection.x, 0f, InputDirection.y) * playerSetting.MoveSpeed * Time.deltaTime;
            if (isAuto)
            {
                targetPosition = autoDirection * playerSetting.MoveSpeed * Time.deltaTime;
            }
            playerSetting.LerpMoveSpeed(true);

            playerTransform.Translate(targetPosition);

            playerObject.rotation = Quaternion.Slerp(playerObject.rotation, Quaternion.LookRotation(targetPosition), Time.deltaTime * playerSetting.RotationSpeed);
        }
        else
        {
            playerSetting.LerpMoveSpeed(false);//Start decreasing speed
        }
    }
    private void HandleAnimation()
    {
        float lerp = Mathf.Lerp(animator.GetFloat("Movement Multiplier"), targetPosition == Vector3.zero ? 0f : isAuto ? 1f : InputDirection.magnitude, Time.deltaTime * 5f);
        animator.SetFloat("Movement Multiplier", lerp);
    }
}