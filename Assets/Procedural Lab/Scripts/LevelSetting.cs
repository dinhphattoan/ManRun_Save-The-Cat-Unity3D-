using System.Collections.Generic;
using System.Linq;
using GameMechanic;
using Unity.AI.Navigation;
using UnityEngine;

public class LevelSetting : MonoBehaviour
{
    public Transform Transform { get; private set; } // Use property for transform
    public int NumberOfRoads;
    public int NumberOfCatSpawners;
    public int NumberOfProb;
    public int NumberOfSamples;

    [Range(0, 1f)]
    [Tooltip("Higher the scale generates more prob on road, this will cause computational expenses")]
    public float DifficultyScale;

    public NavMeshSurface NavMeshSurface;
    public TsunamiLevel[] TsunamiLevels;

    public Transform CatParent;
    public Transform RoadParent;
    public Transform ProbParent;
    public Transform EnvParent;
    public Transform FinishLineTransform;
    public Transform PlayerSpawnPoint;
    public Transform TsunamiSpawnPoint;
    public Transform SpawnedCatHolder;
    public Transform Pivot => transform;
    /// <summary>
    /// This method is used to calculate the difficulty scale, higher the difficulty scale, more probs on road and faster tsunami speed update
    /// </summary>
    /// <param name="roadLength"></param>
    public void CalculateDifficultyScale(float roadLength)
    {
        float roadTotalLength = NumberOfRoads * roadLength;
        // Each 50m of road has 1 prob at hardest level, adjust based on difficulty
        NumberOfProb = Mathf.Clamp((int)(roadTotalLength / (50f - (40f * DifficultyScale))), 0, int.MaxValue);


        NumberOfSamples = (int)roadTotalLength / 10;
        AdjustTsunamiLevels(roadTotalLength);
    }

    public void AdjustTsunamiLevels(float roadLength)
    {
        List<TsunamiLevel> tsunamiLevels = new List<TsunamiLevel>
        {
            TsunamiLevels[0] // Keep the first level
        };

        float meterPerUpdate = 500f; // Base update distance
        meterPerUpdate -= meterPerUpdate * (DifficultyScale / 1.5f);

        for (int i = 1; i < roadLength / meterPerUpdate; i++)
        {
            TsunamiLevel level = new TsunamiLevel();
            float speedUpdate = tsunamiLevels[i - 1].Speed + tsunamiLevels[i - 1].Speed * DifficultyScale / 1.5f;
            level.TriggerTransform = Instantiate(TsunamiLevels[0].TriggerTransform, TsunamiLevels[0].TriggerTransform.parent);
            level.TriggerTransform.name = $"Tsunami Speed: {speedUpdate}";
            level.Speed = speedUpdate;
            level.LevelName = $"Level {i}";
            level.TriggerTransform.position = new Vector3(TsunamiLevels[0].TriggerTransform.position.x, 0, TsunamiLevels[0].TriggerTransform.parent.position.z + meterPerUpdate * i);
            tsunamiLevels.Add(level);
        }

        TsunamiLevels = tsunamiLevels.ToArray();
    }
}