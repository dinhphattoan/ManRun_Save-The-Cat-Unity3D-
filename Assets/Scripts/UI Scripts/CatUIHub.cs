using UnityEngine;
using UnityEngine.UI;

public class CatUIHub : MonoBehaviour
{
    [SerializeField] private CatBehavior catBehavior;
    [SerializeField] private SO_MechanicSetting mechanicSetting;
    [SerializeField] private Transform barkUI;
    [SerializeField] private Transform sliderUI;

    private Slider slider;
    private Transform playerTransform;
    private Transform uiHubTransform;

    private void Start()
    {
        slider = sliderUI.GetComponent<Slider>();
        uiHubTransform = transform;
        playerTransform = GameObject.FindWithTag("Player").transform;

        sliderUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleUI();
        HandleRotation();
    }

    private void HandleUI()
    {
        if (catBehavior.PickupTimeCounter > 0)
        {
            barkUI.gameObject.SetActive(false);
            sliderUI.gameObject.SetActive(true);
            slider.value = catBehavior.PickupTimeCounter / mechanicSetting.PickupTime;
        }
        else
        {
            barkUI.gameObject.SetActive(true);
            sliderUI.gameObject.SetActive(false);
        }
    }

    private void HandleRotation()
    {
        uiHubTransform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
    }
}