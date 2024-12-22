using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CarDatabase", menuName = "Scriptable Objects/SO_CarDatabase")]
public class SO_CarDatabase : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private List<GameObject> _carPrefabs;
    [SerializeField] private List<Material> _carMaterials;

    public GameObject CreateRandomSampleCar(Transform holder)
    {
        GameObject car = Instantiate(_carPrefabs[Random.Range(0, _carPrefabs.Count)], holder);
        RandomizeCarMaterials(car);
        return car;
    }

    private void RandomizeCarMaterials(GameObject car)
    {
        if (TryGetCarParts(car, out GameObject carMesh, out List<GameObject> wheels))
        {
            carMesh.GetComponent<Renderer>().material = _carMaterials[Random.Range(0, _carMaterials.Count)];

            Material wheelMaterial = _carMaterials[Random.Range(0, _carMaterials.Count)];
            foreach (var wheel in wheels)
            {
                wheel.GetComponent<Renderer>().material = wheelMaterial;
            }
        }
    }

    private bool TryGetCarParts(GameObject car, out GameObject carMesh, out List<GameObject> wheels)
    {
        carMesh = null;
        wheels = new List<GameObject>();

        if (car.transform.childCount > 0)
        {
            carMesh = car.transform.GetChild(0).gameObject;

            Transform wheelTransform = carMesh.transform.GetChild(0);
            for (int i = 0; i < wheelTransform.childCount; i++)
            {
                wheels.Add(wheelTransform.GetChild(i).gameObject);
            }

            return true;
        }

        return false;
    }
}

[System.Serializable]
public class Car
{
    public string name;
    public GameObject CarPrefab;
    public bool isBeingUsed = false;
}