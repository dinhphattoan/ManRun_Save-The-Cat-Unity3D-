using UnityEngine;
using UnityEngine.UI;

public class CatUIHub : MonoBehaviour
{
    [SerializeField] private CatBehavior catBehaviorScript;
    [SerializeField] private SO_MechanicSetting mechanicSetting;
    [SerializeField] private Transform catMeowUITransform;
    [SerializeField] private Transform pickupSliderUITransform;

    private Slider pickupSlider;
    private Transform uiHubTransform;

    private void Start()
    {
        pickupSlider = pickupSliderUITransform.GetComponent<Slider>();
        uiHubTransform = transform;

        pickupSliderUITransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleUI();
        HandleRotation();
    }

    private void HandleUI()
    {
        if (catBehaviorScript.PickupTimeCounter > 0)
        {
            catMeowUITransform.gameObject.SetActive(false);
            pickupSliderUITransform.gameObject.SetActive(true);
            pickupSlider.value = catBehaviorScript.PickupTimeCounter / mechanicSetting.PickupTime;
        }
        else
        {
            catMeowUITransform.gameObject.SetActive(true);
            pickupSliderUITransform.gameObject.SetActive(false);
        }
    }

    private void HandleRotation()
    {
        uiHubTransform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
    }
}