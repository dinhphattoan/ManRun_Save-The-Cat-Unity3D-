using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_GameDatabase", menuName = "Scriptable Objects/SO_GameDatabase")]
public class SO_GameDatabase : ScriptableObject
{
    [Header("Player Database")]
    [SerializeField] private int _currencyValue = 1000;
    [SerializeField] private float _maxSpeedAdditionalStored = 0f;

    [Header("Trade Values")]
    [SerializeField] private int _costCurrencyPerUpgrade = 50;
    [SerializeField] private float _speedPerUpgradeEarn = 0.2f;
    [SerializeField] private int _currencyEarn = 12;

    [Header("System Database")]
    [SerializeField] private List<AudioClip> _musicBackgroundClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _musicInGameClips = new List<AudioClip>();
    private int _musicBackgroundIndex = 0;
    private int _musicInGameIndex = 0;

    [Header("Sound Database")]
    [SerializeField] private AudioClip _upgradeClip;
    [SerializeField] private AudioClip _buttonClickClip;
    [SerializeField] private AudioClip _pickupSound;
    [SerializeField] private AudioClip _placeSound;

    public int CurrencyValue { get => _currencyValue; set => _currencyValue = value; }
    public float MaxSpeedAdditionalStored => _maxSpeedAdditionalStored;
    public AudioClip UpgradeClip => _upgradeClip;
    public AudioClip ButtonClickClip => _buttonClickClip;
    public AudioClip PickupSound => _pickupSound;
    public AudioClip PlaceSound => _placeSound;

    // Sound Methods

    public AudioClip PlayInGameClip()
    {
        Initialize();
        return _musicInGameClips[_musicInGameIndex];
    }
    public AudioClip PlayBackgroundClip()
    {
        Initialize();
        return _musicBackgroundClips[_musicInGameIndex];
    }
    public AudioClip NextMusicClip(AudioClip prevClip)
    {
        if (prevClip == null)
        {
            Initialize();
            return _musicBackgroundClips[0];
        }
        if (prevClip == _musicBackgroundClips[_musicBackgroundIndex])
        {
            return NextBackgroundAudioClip();
        }
        if (prevClip == _musicInGameClips[_musicInGameIndex])
        {
            return NextInGameAudioClip();
        }
        Debug.Log("Can't pick the next music background");
        return null;
    }
    public AudioClip NextBackgroundAudioClip()
    {
        _musicBackgroundIndex = (_musicBackgroundIndex+1) % _musicBackgroundClips.Count;
        return _musicBackgroundClips[_musicBackgroundIndex];
    }
    public AudioClip NextInGameAudioClip()
    {
        _musicInGameIndex = (_musicInGameIndex+1) % _musicInGameClips.Count;
        return _musicInGameClips[_musicInGameIndex];
    }
    public AudioClip CurrentBackgroundAudioClip() => _musicBackgroundClips[_musicBackgroundIndex];
    public AudioClip CurrentInGameAudioClip() => _musicInGameClips[_musicInGameIndex];
    // Player Methods
    public bool TradeCurrencyPerMaxSpeed()
    {
        if (_currencyValue < _costCurrencyPerUpgrade)
        {
            return false;
        }

        _currencyValue -= _costCurrencyPerUpgrade;
        _maxSpeedAdditionalStored += _speedPerUpgradeEarn;
        return true;
    }

    public void TradeTapPerCurrency()
    {
        _currencyValue += _currencyEarn;
    }

    public void Initialize()
    {
        _musicBackgroundIndex = 0;
        _musicInGameIndex = 0;
    }
}