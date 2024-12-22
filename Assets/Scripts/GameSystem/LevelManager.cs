using System.Collections.Generic;
using GameMechanic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    [Header("Transforms")]
    public Transform PLAYER_SPAWN_POINT;
    public Transform TSUNAMI_SPAWN_POINT;
    public Transform envHolder;
    public Transform catSpawnerHolder = null;
    public Transform probHolder = null;
    public Transform roadHolder = null;
    public NavMeshSurface roadNavMeshSurface = null;
    [Space]
    public TsunamiLevel[] tsunamiLevels;
    public List<CatSpawner> catSpawners = new List<CatSpawner>();
    public List<Transform> probObjects = new List<Transform>();
    public Transform[] finishTransforms;
    [Header("Level settings")]
    public int numberOfRoad = 5;
    public int numberOfCatSpawner = 5;
    public int numberOfProb = 5;
    public int numberOfSampling = 10;
    private bool isProcess = false;
    public bool IsProcess { get => isProcess; set => isProcess = value; }
    [Header("Transform Placeholder")]
    public Transform spawnedCatHolder = null;
    private GameManager gameManager;
    private ProceduralLevel proceduralLevel;
    private void OnEnable()
    {
        Initialize();
    }
    public void Initialize()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null && PLAYER_SPAWN_POINT != null)
        {
            proceduralLevel = FindFirstObjectByType<ProceduralLevel>();
            //Map the level details
            gameManager.LoadLevelData(this);
            gameManager.Initialize();
        }
    }
    public void GetSpawnedObject()
    {
        for (int i = 0; i < probHolder.childCount; i++)
        {
            probObjects.Add(probHolder.GetChild(i).transform);
        }
    }
    public void AssignLevelSetting(LevelSetting levelSetting)
    {
        numberOfCatSpawner = levelSetting.numCatSpawner;
        TSUNAMI_SPAWN_POINT = levelSetting.tsunamiSpawnPoint;
        PLAYER_SPAWN_POINT = levelSetting.playerSpawnPoint;
        envHolder = levelSetting.envParent;
        numberOfProb = levelSetting.numProb;
        numberOfRoad = levelSetting.numRoads;
        catSpawnerHolder = levelSetting.catParent;
        probHolder = levelSetting.probParent;
        roadHolder = levelSetting.roadParent;
        roadNavMeshSurface = levelSetting.navMeshSurface;
        finishTransforms = new Transform[] { levelSetting.finishLineTransform };
        tsunamiLevels = levelSetting.tsunamiLevels;
        catSpawners = new List<CatSpawner>();
        probObjects = new List<Transform>();
        spawnedCatHolder = levelSetting.spawnedCatHolder;
        for (int i = 0; i < catSpawnerHolder.childCount; i++)
        {
            catSpawners.Add(catSpawnerHolder.GetChild(i).GetComponent<CatSpawner>());
        }

        for (int i = 0; i < probHolder.childCount; i++)
        {
            probObjects.Add(probHolder.GetChild(i).transform);
        }
    }

    public void RandomizeLevel()
    {
        numberOfCatSpawner = (catSpawnerHolder != null) ? catSpawnerHolder.childCount : Random.Range(0, 10);
        numberOfProb = (probHolder != null) ? probHolder.childCount : Random.Range(0, 10);
        numberOfRoad = (roadHolder != null) ? roadHolder.childCount : Random.Range(0, 10);
        ProceduralLevel.ClearChildren(probHolder);
        ProceduralLevel.ClearChildren(catSpawnerHolder);
        ProceduralLevel.ClearChildren(spawnedCatHolder);
        catSpawners.Clear();
        probObjects.Clear();


        isProcess = true;
        proceduralLevel.RandomProbSampling(this);



        for (int i = 0; i < catSpawnerHolder.childCount; i++)
        {
            catSpawners.Add(catSpawnerHolder.GetChild(i).GetComponent<CatSpawner>());
        }

        for (int i = 0; i < probHolder.childCount; i++)
        {
            probObjects.Add(probHolder.GetChild(i).transform);
        }

        isProcess = false;
    }

}
