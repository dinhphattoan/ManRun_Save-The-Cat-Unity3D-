using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class CatSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    SO_MechanicSetting mechanicSetting;
    [SerializeField]
    SO_AnimalDatabase animalDatabase;
    [SerializeField]
    Transform spawnPoint => this.transform;
    [SerializeField]
    public CatType catType;
    
    public Transform SpawnCat(Transform holder)
    {
        Vector3 RandomSpawnPoint = Random.insideUnitSphere * mechanicSetting.SpawnRadius;
        RandomSpawnPoint += spawnPoint.position;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(RandomSpawnPoint, out hit, mechanicSetting.SpawnRadius, NavMesh.AllAreas))
        {
            GameObject cat = Instantiate(animalDatabase.GetCatPrefab(catType), hit.position, Quaternion.identity,holder);
            
            return cat.transform;
        }
        return null;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, mechanicSetting.SpawnRadius);

    }
}
