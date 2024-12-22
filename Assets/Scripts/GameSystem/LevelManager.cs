using System.Collections.Generic;
using GameMechanic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Transforms")]
    public Transform PlayerSpawnPoint;
    public Transform TsunamiSpawnPoint;
    public Transform EnvHolder;
    public Transform CatSpawnerHolder;
    public Transform ProbHolder;
    public Transform RoadHolder;
    public NavMeshSurface RoadNavMeshSurface;

    [Space]
    public TsunamiLevel[] TsunamiLevels;
    public List<CatSpawner> CatSpawners = new List<CatSpawner>();
    public List<Transform> ProbObjects = new List<Transform>();
    public Transform[] FinishTransforms;

    [Header("Level Settings")]
    public int NumberOfRoads = 5;
    public int NumberOfCatSpawners = 5;
    public int NumberOfProb = 5;
    public int NumberOfSampling = 10;
    public bool IsProcess { get; private set; }

    [Header("Transform Placeholder")]
    public Transform SpawnedCatHolder;

    private GameManager gameManager;
    private ProceduralLevel proceduralLevel;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && PlayerSpawnPoint != null)
        {
            proceduralLevel = FindFirstObjectByType<ProceduralLevel>();
            gameManager.LoadLevelData(this);
            gameManager.Initialize();
            FindFirstObjectByType<SoundController>().PlayInGameMusic();
        }
    }

    public void GetSpawnedObject()
    {
        foreach (Transform child in ProbHolder)
        {
            ProbObjects.Add(child);
        }
    }

    public void AssignLevelSetting(LevelSetting levelSetting)
    {
        NumberOfCatSpawners = levelSetting.NumberOfCatSpawners;
        TsunamiSpawnPoint = levelSetting.TsunamiSpawnPoint;
        PlayerSpawnPoint = levelSetting.PlayerSpawnPoint;
        EnvHolder = levelSetting.EnvParent;
        NumberOfProb = levelSetting.NumberOfProb;
        NumberOfRoads = levelSetting.NumberOfRoads;
        CatSpawnerHolder = levelSetting.SpawnedCatHolder;
        ProbHolder = levelSetting.ProbParent;
        RoadHolder = levelSetting.RoadParent;
        RoadNavMeshSurface = levelSetting.NavMeshSurface;
        FinishTransforms = new Transform[] { levelSetting.FinishLineTransform };
        TsunamiLevels = levelSetting.TsunamiLevels;
        CatSpawners = new List<CatSpawner>();
        ProbObjects = new List<Transform>();
        SpawnedCatHolder = levelSetting.SpawnedCatHolder;

        foreach (Transform child in CatSpawnerHolder)
        {
            CatSpawners.Add(child.GetComponent<CatSpawner>());
        }

        foreach (Transform child in ProbHolder)
        {
            ProbObjects.Add(child);
        }
    }

    public void RandomizeLevel()
    {
        NumberOfCatSpawners = CatSpawnerHolder != null ? CatSpawnerHolder.childCount : Random.Range(0, 10);
        NumberOfProb = ProbHolder != null ? ProbHolder.childCount : Random.Range(0, 10);
        NumberOfRoads = RoadHolder != null ? RoadHolder.childCount : Random.Range(0, 10);

        ProceduralLevel.ClearChildren(ProbHolder);
        ProceduralLevel.ClearChildren(CatSpawnerHolder);
        ProceduralLevel.ClearChildren(SpawnedCatHolder);

        CatSpawners.Clear();
        ProbObjects.Clear();

        IsProcess = true;
        proceduralLevel.RandomProbSampling(this);

        foreach (Transform child in CatSpawnerHolder)
        {
            CatSpawners.Add(child.GetComponent<CatSpawner>());
        }

        foreach (Transform child in ProbHolder)
        {
            ProbObjects.Add(child);
        }

        IsProcess = false;
    }
    private void OnDestroy() => FindFirstObjectByType<SoundController>().PlayBackgroundMusic();

}