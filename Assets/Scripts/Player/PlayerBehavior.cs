using System.Collections.Generic;
using UnityEngine;
using GameMechanic;
using System.Linq;
using SaveTheCat;
using UnityEngine.AI;

public class PlayerBehavior : MonoBehaviour
{
    CameraController cameraController;
    private GameManager gameManager;
    [SerializeField] private SO_MechanicSetting playerSetting;
    [Header("Transforms")]
    [SerializeField] private Transform directionArrowPoint;
    [SerializeField] private Transform tsunamiTransform;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform catPlaceholder;
    private Vector3 catPlaceHolderCounter = Vector3.zero;
    [SerializeField] private MeshRenderer hoverMeshRenderer;


    public MeshRenderer meshRenderer { get { return hoverMeshRenderer; } }
    [Header("Cat Objects")]
    [SerializeField] private List<Transform> catHeads = new();
    [SerializeField] private List<Transform> catTails = new();
    public Transform LatestCatTail { get { 
        return catTails.Count == 0 ? playerTransform : catTails[catTails.Count - 1].transform; } }
    [SerializeField] private float catHeadGap = 0.2f;
    [SerializeField] private float distance_cameraUp;
    [SerializeField] private float distance_tsunamiReached;
    private void Update()
    {
        GetNextPickupDirectionPoint();
        // HandleCatTailFollow();
        IsTsunamiReached();
        IsFinishLineReach();
    }

    public void Initialize()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        cameraController = FindFirstObjectByType<CameraController>();
        tsunamiTransform = gameManager.TsunamiTransform;
        //Clear previous
        catHeads.Clear();
        catTails.Clear();
        foreach (Transform cat in catPlaceholder)
        {
            Destroy(cat.gameObject);
        }
    }


    public void Pickup(CatBehavior cat, CatType catType)
    {
        switch (catType)
        {
            case CatType.WhiteOne:
                PlaceCatOnHead(cat.transform, catPlaceholder, catHeads);
                break;
            case CatType.BlackOne:
                Transform prevCat = PlaceCatOnTail(cat.transform);
                if(prevCat == playerTransform)
                {
                    cat.catTailGap =5f;
                }
                else cat.catTailGap = 3f;
                cat.followTargetTransform = prevCat;
                break;
            default:
                Debug.LogError("Unsupported CatType: " + catType);
                break;
        }
        gameManager.RemoveCatTransform(cat.transform);
        if (gameManager.SpawnedCats.Count == 0)
        {
            cameraController.FinishShowcase();
            gameManager.FinalizeWon();
        }
    }

    private void PlaceCatOnHead(Transform cat, Transform parent, List<Transform> catList)
    {
        cat.transform.parent = parent;
        cat.transform.localPosition = catPlaceHolderCounter;
        cat.transform.localRotation = Quaternion.identity;
        catPlaceHolderCounter += Vector3.up * catHeadGap;
        cat.GetComponent<NavMeshAgent>().enabled = false;
        catList.Add(cat.transform);
    }
    private Transform PlaceCatOnTail(Transform cat)
    {
        Transform prevCat = LatestCatTail;
        catTails.Add(cat);
        return prevCat;

    }
    private void GetNextPickupDirectionPoint()
    {
        Transform nearestCat = GetNearestCatTransform();
        if (nearestCat != null && GameManager.GameStart)
        {
            if (!directionArrowPoint.gameObject.activeSelf)
                directionArrowPoint.gameObject.SetActive(true);
            directionArrowPoint.rotation = Quaternion.LookRotation(nearestCat.position - playerTransform.position);
        }
        else
        {
            directionArrowPoint.gameObject.SetActive(false);
        }
    }

    private Transform GetNearestCatTransform()
    {
        if (gameManager.SpawnedCats.Count == 0)
        {
            return null;
        }
        return gameManager.SpawnedCats.ToList().OrderBy(t => t.position.z).First();
    }

    private void IsFinishLineReach()
    {
        if (GameManager.GameStart && playerTransform.position.z >= gameManager.FinishLineTransform.position.z)
        {
            cameraController.FinishShowcase();
            gameManager.FinalizeWon();
        }
    }

    private void IsTsunamiReached()
    {
        Vector3 distance = playerTransform.position - tsunamiTransform.position;
        if (distance.z <= distance_cameraUp)
        {
            cameraController.CamPriorityPlayerUp();
            if (distance.z <= distance_tsunamiReached)
            {
                //Lost
                if (GameManager.GameStart)
                {
                    cameraController.FinishShowcase();
                    gameManager.FinalizeLost();
                }
            }
        }
        else if (GameManager.GameStart)
        {
            cameraController.CamPriorityPlayer();
        }
    }

}