using GameMechanic;
using UnityEngine;

[CreateAssetMenu(fileName = "MechanicSetting", menuName = "SO/MechanicSetting")]
public class SO_MechanicSetting : ScriptableObject
{
    public SO_GameDatabase gameDatabase;
    [Header("Player Settings")]
    [Range(0, 100)]
    [SerializeField] int playerEnergyBar;
    [SerializeField] float tempAdditionalMaxSpeed = 0f;
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float moveSpeedLerpSpeed = 5f;
    public float MaxMoveSpeed { get { return maxMoveSpeed; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float MoveSpeedLerpSpeed { get { return moveSpeedLerpSpeed; } }
    public float RotationSpeed { get; private set; }
    [Space]
    [Header("Tsunami Settings")]
    [SerializeField] private float tsunamiMoveSpeed = 0f;
    public float TsunamiMoveSpeed { get { return tsunamiMoveSpeed; } }

    [Header("Cat")]
    [Space(10)]
    public float SpawnRadius = 20f;
    public float PickupTime = 1f;
    public float MaxDelayWalkingRoutine = 3f;

    [Header("ConeHover")]
    [Space(10)]
    public int Subdivisions;
    public int HoverAngle;
    public float HoverDistance;

    // [Header("Initial")]
    // [Space(10)]
    // [SerializeField] private int initialSecondsPerTile = 5;
    // [SerializeField] private Renderer tileRenderer;

    [Header("Trade Value")]
    [SerializeField] int eneryRecoverPerSecond = 10;
    [SerializeField] private float energyMaxTempEarn = 0.2f;
    [SerializeField] int costEnergyPerSpeed = 12;

    public int PlayerEnergyBar { get => playerEnergyBar; }
    public void Initialize()
    {
        //Intial max movement speed that takes into account the tile size (same with Tsunami)
        //Player need to have higher based speed in order to make progress
        RotationSpeed = 9f;
        playerEnergyBar = 100;
        tempAdditionalMaxSpeed=0f;
        tsunamiMoveSpeed=10f;
    }
    public void TradeTempEnergyPerMaxSpeed()
    {
        playerEnergyBar -= costEnergyPerSpeed;
        tempAdditionalMaxSpeed += energyMaxTempEarn;
    }
    public void RecoverPlayerEnergy()
    {
        playerEnergyBar += eneryRecoverPerSecond;
    }
    public void LerpMoveSpeed(bool isIncrement)
    {
        if (isIncrement)
        {
            moveSpeed =
                    Mathf.Lerp(moveSpeed, isIncrement ?
                    (maxMoveSpeed + tempAdditionalMaxSpeed + gameDatabase.MaxSpeedAdditionalStored) : 0,
                    Time.deltaTime * moveSpeedLerpSpeed);
        }
        else
        {
            moveSpeed =
        Mathf.Lerp(moveSpeed, 0, Time.deltaTime * moveSpeedLerpSpeed);
        }

    }
    public void DifficulityScale(float scale)
    {

    }
    public void SetTsunamiLevel(TsunamiLevel tsunamiLevel)
    {
        tsunamiMoveSpeed = tsunamiLevel.speed;

    }
}