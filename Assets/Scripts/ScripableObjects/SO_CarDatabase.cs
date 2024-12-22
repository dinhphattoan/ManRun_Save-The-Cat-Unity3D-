using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "SO_CarDatabase", menuName = "Scriptable Objects/SO_CarDatabase")]
public class SO_CarDatabase : ScriptableObject
{
    [Header("Data")]
    public List<GameObject> CarPrefabs;
    public List<Material> carMaterials;

    public GameObject CreateRandomSampleCar(Transform holder)
    {
        GameObject car = Instantiate(CarPrefabs[Random.Range(0, CarPrefabs.Count)], holder);

        RandomMaterialsAssign(car);
        return car;
    }

    public void RandomMaterialsAssign(GameObject Car)
    {
        var parts = DissectIntoParts(Car);

        if (parts.Item1 != null || parts.Item2 != null)
        {
            parts.Item1.gameObject.GetComponent<Renderer>().material = carMaterials[Random.Range(0, carMaterials.Count)];
            Material wheelmaterial = carMaterials[Random.Range(0, carMaterials.Count)];
            for (int j = 0; j < parts.Item2.Count; j++)
            {
                parts.Item2[j].GetComponent<Renderer>().material = wheelmaterial;
            }
        }
    }

    public (GameObject, List<GameObject>) DissectIntoParts(GameObject car)
    {
        List<GameObject> parts = new List<GameObject>();

        if (car.transform.childCount > 0)
        {
            GameObject carMesh = car.transform.GetChild(0).gameObject;

            // Add wheels
            Transform wheelTransform = carMesh.transform.GetChild(0);
            for (int i = 0; i < wheelTransform.childCount; i++)
            {
                parts.Add(wheelTransform.GetChild(i).gameObject);
            }

            return (carMesh, parts);
        }
        else
        {
            return (null, null);
        }
    }
}

[System.Serializable]
public class Car
{
    public string name;
    public GameObject CarPrefab;
    public bool isBeingUsed = false;


}