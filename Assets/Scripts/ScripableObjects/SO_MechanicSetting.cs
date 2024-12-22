using GameMechanic;
using UnityEngine;

[CreateAssetMenu(fileName = "MechanicSetting", menuName = "SO/MechanicSetting")]
public class SO_MechanicSetting : ScriptableObject
{
    [Header("Game Database")]
    public SO_GameDatabase gameDatabase;

    [Header("Player Settings")]
    [Range(0, 100)]
    [SerializeField] private int _playerEnergyBar = 100;
    [SerializeField] private float _tempAdditionalMaxSpeed = 0f;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxMoveSpeed = 8f;
    [SerializeField] private float _moveSpeedLerpSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 9f; 

    [Header("Tsunami Settings")]
    [SerializeField] private float _tsunamiMoveSpeed = 10f;

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

    [Header("Trade Value")]
    [SerializeField] private int _eneryRecoverPerSecond = 10;
    [SerializeField] private float _energyMaxTempEarn = 0.2f;
    [SerializeField] private int _costEnergyPerSpeed = 12;

    // Properties
    public int PlayerEnergyBar => _playerEnergyBar;
    public float MaxMoveSpeed => _maxMoveSpeed;
    public float MoveSpeed => _moveSpeed;
    public float MoveSpeedLerpSpeed => _moveSpeedLerpSpeed;
    public float RotationSpeed => _rotationSpeed;
    public float TsunamiMoveSpeed => _tsunamiMoveSpeed;

    // Initialization
    public void Initialize()
    {
        _playerEnergyBar = 100;
        _tempAdditionalMaxSpeed = 0f;
        _tsunamiMoveSpeed=10f;
    }

    // Energy Management
    public void TradeTempEnergyPerMaxSpeed()
    {
        _playerEnergyBar -= _costEnergyPerSpeed;
        _tempAdditionalMaxSpeed += _energyMaxTempEarn;
        ClampPlayerEnergy();
    }

    public void RecoverPlayerEnergy()
    {
        _playerEnergyBar += _eneryRecoverPerSecond;
        ClampPlayerEnergy();
    }

    private void ClampPlayerEnergy()
    {
        _playerEnergyBar = Mathf.Clamp(_playerEnergyBar, 0, 100); 
    }

    // Move Speed Handling
    public void LerpMoveSpeed(bool isIncrement)
    {
        _moveSpeed = Mathf.Lerp(
            _moveSpeed, 
            isIncrement ? 
                (_maxMoveSpeed + _tempAdditionalMaxSpeed + gameDatabase.MaxSpeedAdditionalStored) : 
                0, 
            Time.deltaTime * _moveSpeedLerpSpeed);
    }

    // Difficulty Scaling (Placeholder)
    public void DifficulityScale(float scale) { } 

    // Tsunami Level Setting
    public void SetTsunamiLevel(TsunamiLevel tsunamiLevel)
    {
        _tsunamiMoveSpeed = tsunamiLevel.Speed;
    }
}