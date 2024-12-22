using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnvironmentDatabase", menuName = "Scriptable Objects/SO_EnvironmentDatabase")]
public class SO_EnvironmentDatabase : ScriptableObject
{
    [SerializeField] private GameObject[] environmentPrefabs;
    [SerializeField] private GameObject[] crossRoadPrefabs;
    public GameObject GetCrossRoadPrefab(int index) => crossRoadPrefabs[index];

    public GameObject GetRandomCrossRoadPrefab() => crossRoadPrefabs[Random.Range(0, crossRoadPrefabs.Length)];
    public GameObject GetEnvironmentPrefab(int index) => environmentPrefabs[index];

    public GameObject GetRandomEnvironmentPrefab() => environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];
}

