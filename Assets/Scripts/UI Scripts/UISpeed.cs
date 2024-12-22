using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpeed : MonoBehaviour
{
    [SerializeField]private SO_MechanicSetting playerSetting;
    [SerializeField] private SO_GameDatabase gameDatabase;
    [SerializeField] Image minSlider;
    [SerializeField] TextMeshProUGUI minTextPro;
    [SerializeField] TextMeshProUGUI maxTextPro;
    float baseMaxSpeed=0f;

    void Start()
    {
        baseMaxSpeed = playerSetting.MaxMoveSpeed;
    }

    void Update() =>HandleUISlide();
    

    void HandleUISlide()
    {
        float nextValue = Mathf.Clamp((playerSetting.MoveSpeed/baseMaxSpeed)/2f, 0, 0.5f);
        minSlider.fillAmount = nextValue;
        minTextPro.text = playerSetting.MoveSpeed.ToString("0.0");
        maxTextPro.text = (playerSetting.MaxMoveSpeed + gameDatabase.MaxSpeedAdditionalStored).ToString("0.0");
    }
}
