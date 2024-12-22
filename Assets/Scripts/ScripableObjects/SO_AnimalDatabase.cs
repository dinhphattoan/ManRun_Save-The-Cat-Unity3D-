using UnityEngine;

[CreateAssetMenu(fileName = "SO_AnimalDatabase", menuName = "Scriptable Objects/SO_AnimalDatabase")]
public class SO_AnimalDatabase : ScriptableObject
{
    [SerializeField] private Cat[] cats;

    [Header("Navigation Settings")]
    public float WalkSpeed = 1f;
    public float WalkDistanceRadius = 20f;
    public GameObject GetRandomCatPrefab() => cats[Random.Range(0, cats.Length)].prefab;
    public GameObject GetCat(int index) => cats[index].prefab;
    [SerializeField] AudioClip[] catSounds;
    public GameObject GetCatPrefab(CatType catType = default)
    {
        return cats[(int)catType].prefab;
    }
    public AudioClip GetRandomCatSoundClip() => catSounds[Random.Range(0, catSounds.Length)];
}

[System.Serializable]
public class Cat
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