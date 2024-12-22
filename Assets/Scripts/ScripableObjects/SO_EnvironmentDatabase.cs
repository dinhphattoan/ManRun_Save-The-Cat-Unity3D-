using UnityEngine;

[CreateAssetMenu(fileName = "SO_EnvironmentDatabase", menuName = "Scriptable Objects/SO_EnvironmentDatabase")]
public class SO_EnvironmentDatabase : ScriptableObject
{
    [SerializeField] private GameObject[] _environmentPrefabs;
    [SerializeField] private GameObject[] _crossRoadPrefabs;

    public GameObject GetCrossRoadPrefab(int index) 
    {
        if (index >= 0 && index < _crossRoadPrefabs.Length) 
        {
            return _crossRoadPrefabs[index];
        }
        Debug.LogError($"Invalid index: {index}. Index out of bounds for crossRoadPrefabs.");
        return null; 
    }

    public GameObject GetRandomCrossRoadPrefab() 
    {
        if (_crossRoadPrefabs.Length > 0) 
        {
            return _crossRoadPrefabs[Random.Range(0, _crossRoadPrefabs.Length)];
        }
        Debug.LogError("No crossRoadPrefabs found in the database.");
        return null; 
    }

    public GameObject GetEnvironmentPrefab(int index) 
    {
        if (index >= 0 && index < _environmentPrefabs.Length) 
        {
            return _environmentPrefabs[index];
        }
        Debug.LogError($"Invalid index: {index}. Index out of bounds for environmentPrefabs.");
        return null; 
    }

    public GameObject GetRandomEnvironmentPrefab() 
    {
        if (_environmentPrefabs.Length > 0) 
        {
            return _environmentPrefabs[Random.Range(0, _environmentPrefabs.Length)];
        }
        Debug.LogError("No environmentPrefabs found in the database.");
        return null; 
    }
}