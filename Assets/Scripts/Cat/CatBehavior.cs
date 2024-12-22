using System.Collections;
using System.Linq;
using GameMechanic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class CatBehavior : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SO_AnimalDatabase animalDatabase;
    [SerializeField] private SO_MechanicSetting mechanicSetting;
    [Header("References")]
    private ScriptHolder scriptHolder;
    [SerializeField] private CatType catType;
    [SerializeField] private CatUIHub catUIHub;
    public float catTailGap { private get; set; }
    private GameManager gameManager;
    Animator catAnimator;

    private CatBehavior catBehavior;
    private Transform catTransform;
    private Transform playerTransform;
    public Transform followTargetTransform { set; private get; }
    private NavMeshAgent navMeshAgent;
    public CatType Type => catType;
    public float PickupTimeCounter { get; private set; }

    bool IsContractHover;
    bool isPicked = false;
    Vector3 targetNavPoint;
    Vector3 saveTargetNavPoint;
    bool forceNavPoint = false;
    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        catBehavior = this;
        gameManager = FindFirstObjectByType<GameManager>(); ;
        catTransform = transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        catAnimator = GetComponentInChildren<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        scriptHolder = GameObject.FindFirstObjectByType<ScriptHolder>();
        StartCoroutine(RandomWalkRoutine());
    }
    private void Update()
    {
        HandleAnimation();
    }
    private void HandleAnimation()
    {
        if (navMeshAgent.enabled)
        {
            UpdatePickupTimer();
            if (navMeshAgent.enabled && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                catAnimator.SetBool("IsRun", true);
                return;
            }

        }
        else
        {
            if (followTargetTransform != null)
            {
                Vector3 tailPosition = (followTargetTransform.position - catTransform.position).normalized;
                catTransform.rotation =
                 Quaternion.Lerp(catTransform.rotation, Quaternion.LookRotation(tailPosition), Time.deltaTime * mechanicSetting.MoveSpeed);

                if (Vector3.Distance(followTargetTransform.position, catTransform.transform.position) <= catTailGap)
                    tailPosition = Vector3.zero;

                catTransform.position = Vector3.Lerp(catTransform.position, catTransform.position + tailPosition, Time.deltaTime * mechanicSetting.MoveSpeed);
                catAnimator.SetBool("IsRun", true);
                return;
            }

        }
        catAnimator.SetBool("IsRun", false);
    }

    public Vector2 GetTargetPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * animalDatabase.WalkDistanceRadius;
        return randomOffset + new Vector2(transform.position.x, transform.position.z);
    }

    private IEnumerator RandomWalkRoutine()
    {
        targetNavPoint = transform.position;
        while (navMeshAgent.enabled)
        {
            yield return new WaitForSeconds(Random.Range(1f, mechanicSetting.MaxDelayWalkingRoutine));
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !forceNavPoint)
            {
                if (RandomPoint(transform.position, animalDatabase.WalkDistanceRadius, out targetNavPoint))
                {
                    saveTargetNavPoint = targetNavPoint;
                    navMeshAgent.speed = animalDatabase.WalkSpeed;
                    navMeshAgent.SetDestination(targetNavPoint);
                    RandomBarkRoutine();
                }
            }
        }
    }
    private void RandomBarkRoutine()
    {
        if (Random.Range(0, 2) == 0)
        {
            scriptHolder.SoundController.PlayAudioClip(animalDatabase.GetRandomCatSoundClip(), Vector3.Distance(catTransform.position, playerTransform.position) / 2);
        }
    }
    private void HandlePickup()
    {
        StopAllCoroutines();
        navMeshAgent.enabled = false;
        gameManager.PlayerBehavior.Pickup(catBehavior, catType);
        scriptHolder.SoundController.PlayAudioClip(gameManager.GameDatabase.PickupSound, 0);
        isPicked = true;
        IsContractHover = false;
        forceNavPoint = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>().meshRenderer.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        catUIHub.gameObject.SetActive(false);
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        for (int i = 0; i < 30; i++)
        {
            NavMeshHit hit;
            if (randomPoint.x < 100 && randomPoint.x > 0 && randomPoint.z <= gameManager.FinishLineTransform.position.z)
            {
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }

            randomPoint = center + Random.insideUnitSphere * range;
        }
        result = Vector3.zero;
        return false;
    }
    private void UpdatePickupTimer()
    {
        if (IsContractHover)
        {
            PickupTimeCounter = Mathf.MoveTowards(PickupTimeCounter, mechanicSetting.PickupTime, Time.deltaTime);
        }


        if (PickupTimeCounter >= mechanicSetting.PickupTime)
        {
            HandlePickup();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ContactHover") && GameManager.GameStart)
        {
            IsContractHover = true;
            other.GetComponent<MeshRenderer>().enabled = true;
            var SO_playersetting = playerTransform.GetComponent<PlayerController>().PlayerSetting;
            if (SO_playersetting)
            {
                forceNavPoint = true;
                targetNavPoint = Vector3.forward * 1000;
                navMeshAgent.speed = 9f;
                navMeshAgent.SetDestination(targetNavPoint);
            }
            else
            {
                Debug.LogError("No PlayerSetting Scriptable Object found");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ContactHover") && GameManager.GameStart)
        {
            IsContractHover = false;
            forceNavPoint = false;
            other.GetComponent<MeshRenderer>().enabled = false;
            //Repick the navigation point
            if (!isPicked)
            {
                navMeshAgent.SetDestination(saveTargetNavPoint);
                navMeshAgent.speed = animalDatabase.WalkSpeed;
                PickupTimeCounter = 0;

            }

        }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireSphere(transform.position, animalDatabase.WalkDistanceRadius);
    // }

}