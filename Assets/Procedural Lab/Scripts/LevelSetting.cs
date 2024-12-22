using System.Collections.Generic;
using System.Linq;
using GameMechanic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public Transform pivot => transform;
    public int numRoads;
    public int numCatSpawner;
    public int numProb;
    public int numSample;
    [Range(0, 1f)]
    [Tooltip("Higher the scale generate more prob on road, this will cause computational expenses")]
    public float difficulityScale;
    public NavMeshSurface navMeshSurface;
    public TsunamiLevel[] tsunamiLevels;

    public Transform catParent;
    public Transform roadParent;
    public Transform probParent;
    public Transform envParent;
    public Transform finishLineTransform;
    public Transform playerSpawnPoint;
    public Transform tsunamiSpawnPoint;
    public Transform spawnedCatHolder;
    public void ClearChildren()
    {
        ProceduralLevel.ClearChildren(catParent);
        ProceduralLevel.ClearChildren(roadParent);
        ProceduralLevel.ClearChildren(probParent);
        ProceduralLevel.ClearChildren(envParent);
    }


    public void DifficulityScale(float roadLength, SO_MechanicSetting sO_MechanicSetting)
    {
        float roadTotalLength = numRoads * roadLength;
        //Each 50m of road has 1 prob at hardless level, make it more difficult to navigate
        numProb = (int)(roadTotalLength / (difficulityScale == 0 ? 50f : 50f - (40f * difficulityScale)));

        // numCatSpawner = (int)(roadTotalLength/ (difficulityScale == 0 ? 100f : 100f - (50f * difficulityScale)));
        numSample = ((int)roadTotalLength) / 10;
        TsunamiLevelMeasurement(numRoads, numRoads * roadLength, sO_MechanicSetting);
    }
    /// <summary>
    /// Higher the scale, tsunami will likely catchup.
    ///  Tsunami will update it's speed more fluently and depend on the distance speed from tsunami and players, 
    ///  Player have to less time to pickup and have to maintain the navigation through the prob more efficiently and optimized.
    /// </summary>
    /// <param name="numRoads"></param>
    /// <param name="roadLength"></param>
    /// <param name="sO_MechanicSetting"></param>
    void TsunamiLevelMeasurement(int numRoads, float roadLength, SO_MechanicSetting sO_MechanicSetting)
    {
        for (int i = 1; i < tsunamiLevels.Length; i++)
        {
            DestroyImmediate(tsunamiLevels[i].triggerTransform.gameObject);

        }
        TsunamiLevel temp = tsunamiLevels[0];
        List<TsunamiLevel> _tsunamiLevel = new List<TsunamiLevel>();
        _tsunamiLevel.Add(temp);
        tsunamiLevels[0] = temp;
        //More harder makes tsunami update it's speed more fluently
        Transform trigger = _tsunamiLevel[0].triggerTransform;
        float meterPerUpdate = 500; //Default level update per distance
        meterPerUpdate -= meterPerUpdate * (difficulityScale/1.5f);
        for (int i = 1; i < roadLength / meterPerUpdate; i++)
        {
            TsunamiLevel tsunamiLevel = new TsunamiLevel();
            float speedUpdate = _tsunamiLevel[i - 1].speed + _tsunamiLevel[i - 1].speed * difficulityScale/1.5f;
            tsunamiLevel.triggerTransform = Instantiate(trigger, trigger.parent);
            tsunamiLevel.triggerTransform.name = $"Tsunami Speed: " + speedUpdate;
            tsunamiLevel.speed = speedUpdate;
            tsunamiLevel.levelName = "Level " + i;
            tsunamiLevel.triggerTransform.position = new Vector3(trigger.position.x, 0, trigger.parent.position.z + meterPerUpdate * i);
            _tsunamiLevel.Add(tsunamiLevel);
        }
        tsunamiLevels = _tsunamiLevel.ToArray();

    }
}