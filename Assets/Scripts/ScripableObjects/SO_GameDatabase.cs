using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_GameDatabase", menuName = "Scriptable Objects/SO_GameDatabase")]
public class SO_GameDatabase : ScriptableObject
{
    [Header("Player database")]

    [SerializeField] int currencyValue = 1000;
    public int CurrencyValue { get => currencyValue; set => currencyValue = value; }
    [SerializeField] float maxSpeedAdditionalStored = 0f;
    public float MaxSpeedAdditionalStored { get => maxSpeedAdditionalStored; }

    [Header("Trade Value")]
    [SerializeField] int costCurrencyPerUpgrade = 50;
    [SerializeField] float speedPerUpgradeEarn = 0.2f;
    [SerializeField] int currencyEarn = 12;
    [Space(20)]
    [Header("System database")]
    [SerializeField] List<AudioClip> musicBackgroundClips = new();
    private int musicClipIndex = 0;
    [Header("Sound database")]
    
    [SerializeField]  AudioClip upgradeClip;
    public  AudioClip UpgradeClip { get => upgradeClip; }
    [SerializeField]  AudioClip buttonClickClip;
    public  AudioClip ButtonClickClip { get => buttonClickClip; }

    [SerializeField] AudioClip pickupSound;
    public AudioClip PickupSound { get => pickupSound; }
    [SerializeField] AudioClip placeSound;
    [Header("Player steps")]
    [SerializeField] AudioClip[] stepLeft;
    [SerializeField] AudioClip[] stepRight;
    public AudioClip PlaceSound { get => placeSound; }
    public void Initialize()
    {
        musicClipIndex=0;
        
    }
    public bool TradeCurrencyPerMaxSpeed()
    {
        if (currencyValue < costCurrencyPerUpgrade) return false;
        currencyValue -= costCurrencyPerUpgrade;
        maxSpeedAdditionalStored += speedPerUpgradeEarn;
        return true;
    }
    public void TradeTapPerCurrency()
    {
        currencyValue += currencyEarn;
    }
    public AudioClip NextBackgroundAudioClip() 
    {
        return musicBackgroundClips[++musicClipIndex % musicBackgroundClips.Count];
    }
    public AudioClip CurrentBackgroundAudioClip() => musicBackgroundClips[musicClipIndex];

}
