using UnityEngine;

[CreateAssetMenu(fileName = "SO_AnimalDatabase", menuName = "Scriptable Objects/SO_AnimalDatabase")]
public class SO_AnimalDatabase : ScriptableObject
{
    [SerializeField] private CatObject[] _catObjects;

    [Header("Navigation Settings")]
    public float CatWalkSpeed = 1f;
    public float CatWalkDistanceRadius = 20f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _catSounds;

    // Public Getters
    public GameObject GetRandomCatPrefab() => _catObjects[Random.Range(0, _catObjects.Length)].prefab;
    public GameObject GetCatPrefab(int index) => _catObjects[index].prefab;
    public GameObject GetCatPrefab(CatType catType) => GetCatObjectByType(catType)?.prefab ?? null; 
    public AudioClip GetRandomCatSoundClip() => _catSounds[Random.Range(0, _catSounds.Length)];

    private CatObject GetCatObjectByType(CatType catType) 
    {
        foreach (var catObject in _catObjects)
        {
            if (catObject.Type == catType)
            {
                return catObject;
            }
        }
        return null; 
    }
}

[System.Serializable]
public class CatObject
{
    public GameObject prefab;
    public string Name;
    public CatType Type;
}

public enum CatType
{
    WhiteOne,
    BlackOne
}