using System.Collections;
using System.Collections.Generic;
using GameMechanic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ProceduralLevel : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public GameObject LevelPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject foundationPrefab;
    [SerializeField] private GameObject catSpawner;
    [SerializeField] private SO_CarDatabase carDatabase;
    [SerializeField] private SO_MechanicSetting mechanicSetting;
    [SerializeField] private SO_EnvironmentDatabase environmentDatabase;

    [Header("Transforms")]
    [SerializeField] private Transform levelParent;
    [SerializeField] static private float probRadius = 20f;
    public Transform[] levelSettingTransforms;
    public LevelSetting[] LevelSettings;

    //Parameters
    private float roadLength;
    private float foundationLength;

    public GameObject[] Initialize(int numOfLevel)
    {
        LevelSettings = new LevelSetting[numOfLevel];
        levelSettingTransforms = new Transform[numOfLevel];

        roadLength = roadPrefab.GetComponent<MeshRenderer>().bounds.size.z;
        foundationLength = foundationPrefab.GetComponent<MeshRenderer>().bounds.size.z;

        ClearChildren(levelParent);
        GameObject[] go = new GameObject[numOfLevel];
        for (int i = 0; i < numOfLevel; i++)
        {
            GameObject levelObject = Instantiate(LevelPrefab, levelParent.position, Quaternion.identity, levelParent);
            levelObject.name = "Level " + i;
            levelObject.transform.position = new Vector3(350 * i, 0, 0);
            LevelSetting levelSetting = levelObject.GetComponent<LevelSetting>();

            levelSetting.NumberOfRoads = 10;

            LevelSettings[i] = levelSetting;
            InstantiateRoads(levelSetting);
            levelSettingTransforms[i] = levelObject.transform;
            LevelManager levelManager = levelObject.AddComponent<LevelManager>();
            levelManager.AssignLevelSetting(levelSetting);
            go[i] = levelObject;
        }
        return go;
    }
    public void UnloadLevel()
    {
        ClearChildren(levelParent);
        LevelSettings = null;
    }
    public void Reinitialize()
    {
        if (LevelSettings.Length == 0)
        {
            Debug.Log("There's is no level to reinitialize");
            return;
        }
        roadLength = roadPrefab.GetComponent<MeshRenderer>().bounds.size.z;
        for (int i = 0; i < LevelSettings.Length; i++)
        {
            GameObject levelObject = LevelSettings[i].gameObject;
            levelObject.name = "Level " + i;
            levelObject.transform.position = new Vector3(350 * i, 0, 0);
            LevelSetting levelSetting = levelObject.GetComponent<LevelSetting>();

            ClearChildren(levelSetting.CatParent);
            ClearChildren(levelSetting.RoadParent);
            ClearChildren(levelSetting.ProbParent);
            ClearChildren(levelSetting.EnvParent);

            InstantiateRoads(levelSetting);
        }
    }

    #region Generator
    /// <summary>
    /// Random sampling for reload level
    /// </summary>
    /// <param name="levelManager"></param>
    public void RandomProbSampling(LevelManager levelManager)
    {
        roadLength = roadPrefab.GetComponent<MeshRenderer>().bounds.size.z;
        List<Transform> catSpawners = new List<Transform>();
        Vector3 bound = new Vector3(roadLength, 0, roadLength * levelManager.NumberOfRoads);
        float roadDistance = bound.z;
        float spawnRegionDivision = roadDistance / levelManager.NumberOfCatSpawners;
        float leftBound = levelManager.EnvHolder.position.x - (bound.x / 2f) + 10;
        float rightBound = levelManager.EnvHolder.position.x + (bound.x / 2f) - 10;
        int enumLength = System.Enum.GetNames(typeof(CatType)).Length;

        for (int i = 0; i < levelManager.NumberOfCatSpawners; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(leftBound, rightBound), 0, Random.Range(spawnRegionDivision * (i), spawnRegionDivision * (i + 1)));
            GameObject spawner = Instantiate(catSpawner, spawnPosition, Quaternion.identity, levelManager.CatSpawnerHolder);
            spawner.GetComponent<CatSpawner>().catType = (CatType)Random.Range(0, enumLength);
            catSpawners.Add(spawner.transform);
        }
        spawnRegionDivision = roadDistance / levelManager.NumberOfProb;
        for (int i = 0; i < levelManager.NumberOfProb; i++)
        {
            int k = 0;
            while (k++ < 30)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(leftBound, rightBound), 0, Random.Range(spawnRegionDivision * (i), spawnRegionDivision * (i + 1)));
                bool flag = false;
                for (int j = 0; j < catSpawners.Count; j++)
                {
                    if (Vector3.Distance(spawnPosition, catSpawners[j].position) <= (mechanicSetting.SpawnRadius))
                    {
                        flag = true; break;
                    }
                }
                for (int j = 0; j < levelManager.ProbObjects.Count; j++)
                {
                    if (Vector3.Distance(spawnPosition, levelManager.ProbObjects[j].position) <= probRadius)
                    {
                        flag = true; break;
                    }
                }
                if (!flag)
                {
                    GameObject car = carDatabase.CreateRandomSampleCar(levelManager.ProbHolder);
                    car.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                    car.transform.position = spawnPosition;
                    levelManager.ProbObjects.Add(car.transform);
                    break;
                }
            }
        }
        levelManager.ProbHolder.parent = levelManager.RoadHolder; //Asign parent to bake navmesh hierarchy
        levelManager.RoadNavMeshSurface.BuildNavMesh();
        levelManager.ProbHolder.parent = levelManager.RoadHolder.parent;
    }

    void InstantiateRoads(LevelSetting levelSetting)
    {
        levelSetting.CalculateDifficultyScale(roadLength);
        Transform pivot = levelSetting.Pivot;
        int numberOfRoads = levelSetting.NumberOfRoads;
        Transform envParent = levelSetting.EnvParent;
        Transform roadParent = levelSetting.RoadParent;
        NavMeshSurface roadNavMeshSurface = levelSetting.NavMeshSurface;

        Vector3 roadPosition = pivot.position;
        Vector3 foundationPosition = new Vector3(0, 0, 50f);
        Quaternion rotation = Quaternion.identity;
        for (int i = 0; i < numberOfRoads + 5; i++)
        {
            Vector3 leftPosition = foundationPosition + new Vector3(pivot.position.x, 0, pivot.position.z + (foundationLength * i)) + Vector3.left * (roadLength / 2f + foundationLength / 2f);
            Vector3 rightPosition = foundationPosition + new Vector3(pivot.position.x, 0, pivot.position.z + (foundationLength * i)) + Vector3.right * (roadLength / 2f + foundationLength / 2f);
            Instantiate(foundationPrefab, leftPosition, rotation, envParent);
            Instantiate(foundationPrefab, rightPosition, rotation, envParent);

            roadPosition = new Vector3(pivot.position.x, 0, pivot.position.z + roadLength * i);
            GameObject road = Instantiate(roadPrefab, roadPosition, rotation, roadParent);
            road.transform.position = roadPosition;
            road.transform.Rotate(Vector3.up, 90f);
        }

        //Probs sampling
        ProbRoadSampling(levelSetting);
        levelSetting.ProbParent.parent = levelSetting.RoadParent;//Asign parent to bake navmesh hierarchy
        roadNavMeshSurface.BuildNavMesh();
        levelSetting.ProbParent.parent = levelSetting.RoadParent.parent;

        //Environment sampling
        float roadLeftBound = pivot.position.x - roadLength / 2f;
        float roadRightBound = pivot.position.x + roadLength / 2f;
        float leftBound = roadLeftBound - (foundationLength / 2f);
        float rightBound = roadRightBound + (foundationLength / 2f);
        float distanceBound = pivot.position.z + (roadLength * numberOfRoads);
        for (int i = 0; i < levelSetting.NumberOfSamples; i++)
        {
            int k = 0;
            while (k < 30)
            {
                Vector3 position = new Vector3(Random.Range(leftBound, rightBound), 0, Random.Range(pivot.position.z, distanceBound));
                if (position.x < roadLeftBound || position.x > roadRightBound)
                {
                    Instantiate(environmentDatabase.GetRandomEnvironmentPrefab(), position, Quaternion.identity, envParent);
                    break;
                }
                k++;
            }

        }

        levelSetting.FinishLineTransform.position = new Vector3(pivot.position.x, 0, pivot.position.z + roadLength * numberOfRoads);

    }
    void ProbRoadSampling(LevelSetting levelSetting)
    {
        int numberOfAnimals = levelSetting.NumberOfCatSpawners;
        int numberOfCars = levelSetting.NumberOfProb;

        Transform roadParent = levelSetting.RoadParent;
        Transform catSpawnerParent = levelSetting.CatParent;
        Transform probParent = levelSetting.ProbParent;
        List<Vector3> probObjects = new List<Vector3>();
        List<Vector3> spanwedCats = new List<Vector3>();

        float roadDistance = roadLength * levelSetting.NumberOfRoads;
        float spawnRegionDivision = roadDistance / numberOfAnimals;
        float leftBound = roadParent.position.x - (roadLength / 2f) + 10;
        float rightBound = roadParent.position.x + (roadLength / 2f) - 10;
        int enumLength = System.Enum.GetNames(typeof(CatType)).Length;

        //Cat spawner sampling
        for (int i = 0; i < numberOfAnimals; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(leftBound, rightBound), 0, levelSetting.Pivot.position.z + Random.Range(spawnRegionDivision * (i), spawnRegionDivision * (i + 1)));
            GameObject spanwer = Instantiate(catSpawner, spawnPosition, Quaternion.identity, catSpawnerParent);
            spanwer.name = "Spawner";
            spanwer.GetComponent<CatSpawner>().catType = (CatType)Random.Range(0, enumLength);
            spanwedCats.Add(spanwer.transform.position);
        }


        //Sampling prob on road
        spawnRegionDivision = roadDistance / numberOfCars;
        for (int i = 0; i < numberOfCars; i++)
        {
            int k = 0;
            while (k++ < 30)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(leftBound, rightBound), 0, levelSetting.Pivot.position.z + Random.Range(spawnRegionDivision * (i), spawnRegionDivision * (i + 1)));
                bool flag = false;
                if (Vector3.Distance(levelSetting.Pivot.transform.position, spawnPosition) < 20)
                {
                    flag = true; break;
                }
                for (int j = 0; j < spanwedCats.Count; j++)
                {
                    if (Vector3.Distance(spawnPosition, spanwedCats[j]) <= mechanicSetting.SpawnRadius)
                    {
                        flag = true; break;
                    }
                }
                for (int j = 0; j < probObjects.Count; j++)
                {
                    if (Vector3.Distance(spawnPosition, probObjects[j]) <= probRadius)
                    {
                        flag = true; break;
                    }
                }
                if (!flag)
                {
                    GameObject car = carDatabase.CreateRandomSampleCar(probParent);
                    car.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
                    car.transform.position = spawnPosition;
                    probObjects.Add(car.transform.position);
                    break;
                }
            }

        }
    }

    #endregion
    public static void ClearChildren(Transform parent)
    {
        if (parent == null) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }

}

